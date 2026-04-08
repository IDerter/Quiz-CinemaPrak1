using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections;
using System;

namespace QuizCinema
{
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollRectSnap : MonoBehaviour, IEndDragHandler, IBeginDragHandler
    {
        public event Action OnSnapChangedByUser;

        [Header("Snapping")]
        [SerializeField] private float _snapSpeed = 10f;
        [SerializeField] private float _minSwipeThreshold = 50f; // Минимальное расстояние свайпа для авто-докрутки
        [SerializeField] private float _velocityThreshold = 100f; // Порог скорости для определения направления

        [Header("References")]
        [Tooltip("The content panel that holds the items.")]
        [SerializeField] private RectTransform _contentPanel;

        private RectTransform[] _items;
        private ScrollRect _scrollRect;
        private bool _isSnapping = false;
        private bool _isInitialized = false;
        private int _currentItemIndex = 0;
        private Vector2 _dragStartPosition;
        private Vector2 _dragEndPosition;
        private float _dragStartTime;
        private float _dragEndTime;

        private void Awake()
        {
            _scrollRect = GetComponent<ScrollRect>();
        }

        public void Initialize(RectTransform[] newItems)
        {
            if (newItems == null || newItems.Length == 0)
            {
                _isInitialized = false;
                return;
            }
            _items = newItems;
            _isInitialized = true;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!_isInitialized) return;

            DOTween.Kill(_contentPanel);
            _isSnapping = false;

            // Запоминаем начальную позицию и время
            _dragStartPosition = eventData.position;
            _dragStartTime = Time.time;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_isInitialized || _isSnapping) return;

            // Запоминаем конечную позицию и время
            _dragEndPosition = eventData.position;
            _dragEndTime = Time.time;

            // Вычисляем дистанцию и время свайпа
            float dragDistance = Vector2.Distance(_dragStartPosition, _dragEndPosition);
            float dragDuration = _dragEndTime - _dragStartTime;
            float dragVelocity = dragDistance / dragDuration;

            // Находим ближайший элемент
            int closestIndex = FindClosestItemIndex();

            // Если свайп был быстрым и достаточно длинным, определяем направление
            if (dragDistance > _minSwipeThreshold && dragVelocity > _velocityThreshold)
            {
                Vector2 dragDirection = (_dragEndPosition - _dragStartPosition).normalized;

                // Для горизонтального скролла
                if (Mathf.Abs(dragDirection.x) > Mathf.Abs(dragDirection.y))
                {
                    if (dragDirection.x > 0 && _currentItemIndex > 0)
                    {
                        // Свайп вправо - двигаем к предыдущему элементу
                        _currentItemIndex--;
                    }
                    else if (dragDirection.x < 0 && _currentItemIndex < _items.Length - 1)
                    {
                        // Свайп влево - двигаем к следующему элементу
                        _currentItemIndex++;
                    }
                }
                // Для вертикального скролла
                else
                {
                    if (dragDirection.y > 0 && _currentItemIndex > 0)
                    {
                        // Свайп вверх - двигаем к предыдущему элементу
                        _currentItemIndex--;
                    }
                    else if (dragDirection.y < 0 && _currentItemIndex < _items.Length - 1)
                    {
                        // Свайп вниз - двигаем к следующему элементу
                        _currentItemIndex++;
                    }
                }

                SnapTo(_items[_currentItemIndex]);
                OnSnapChangedByUser?.Invoke();
            }
            else
            {
                // Обычная докрутка к ближайшему элементу
                if (closestIndex != -1 && closestIndex != _currentItemIndex)
                {
                    _currentItemIndex = closestIndex;
                    SnapTo(_items[_currentItemIndex]);
                    OnSnapChangedByUser?.Invoke();
                }
                else if (closestIndex != -1)
                {
                    // Возвращаем на место
                    SnapTo(_items[_currentItemIndex]);
                }
            }
        }

        private void SnapTo(RectTransform target)
        {
            if (target == null) return;

            _isSnapping = true;

            Vector3 viewportWorldCenter = _scrollRect.viewport.TransformPoint(_scrollRect.viewport.rect.center);
            Vector3 targetWorldCenter = target.TransformPoint(target.rect.center);
            Vector3 difference = viewportWorldCenter - targetWorldCenter;

            Vector3 newPosition = _contentPanel.position + difference;

            _contentPanel.DOMove(newPosition, 1f / _snapSpeed)
                .SetEase(Ease.OutCubic)
                .OnComplete(() => _isSnapping = false);
        }

        private int FindClosestItemIndex()
        {
            if (!_isInitialized) return -1;

            int closestIndex = -1;
            float smallestDistance = float.MaxValue;

            Vector3 viewportWorldCenter = _scrollRect.viewport.TransformPoint(_scrollRect.viewport.rect.center);

            for (int i = 0; i < _items.Length; i++)
            {
                Vector3 itemWorldCenter = _items[i].TransformPoint(_items[i].rect.center);
                float distance = Vector3.Distance(itemWorldCenter, viewportWorldCenter);

                if (distance < smallestDistance)
                {
                    smallestDistance = distance;
                    closestIndex = i;
                }
            }
            return closestIndex;
        }

        public int GetCurrentSnappedIndex()
        {
            return _currentItemIndex;
        }

        public void ResetToStart()
        {
            if (!_isInitialized || _scrollRect == null || _scrollRect.viewport == null) return;

            StopAllCoroutines();
            _contentPanel.DOKill();
            _isSnapping = false;
            _currentItemIndex = 0;

            Vector3 viewportWorldCenter = _scrollRect.viewport.TransformPoint(_scrollRect.viewport.rect.center);
            Vector3 targetWorldCenter = _items[0].TransformPoint(_items[0].rect.center);
            Vector3 difference = viewportWorldCenter - targetWorldCenter;
            _contentPanel.position += difference;
        }

        public void SnapToItem(int index)
        {
            if (!_isInitialized || index < 0 || index >= _items.Length) return;

            _currentItemIndex = index;
            SnapTo(_items[index]);
        }
    }
}