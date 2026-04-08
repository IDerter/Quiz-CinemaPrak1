using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace QuizCinema
{
    // Убираем RequireComponent, так как основная логика теперь не на этой картинке
    public class SwipeLvl : MonoBehaviour
    {
        [Header("Snapping Logic")]
        [SerializeField] private ScrollRectSnap _scrollRectSnap;

        [SerializeField] private RectTransform[] _lvls;

        [Header("UI References")]
        [SerializeField] private Image[] _visulationCadrIndex; // Точки-индикаторы

        [SerializeField] private int _currentIndex = 0; // Начинаем с 0, чтобы гарантировать обновление при первом кадре
        private bool _isInitialized = false;
        
        private const string _swipeSFX = "Swipe"; // Можно оставить, если ScrollRectSnap будет проигрывать звук

        private void Awake()
        {
            if (_scrollRectSnap != null)
            {
                _scrollRectSnap.OnSnapChangedByUser += PlaySwipeSound;
            }

            if (gameObject.activeInHierarchy)
            {
                SetupSwipe();
            }
        }

        private void OnDestroy()
        {
            if (_scrollRectSnap != null)
            {
                _scrollRectSnap.OnSnapChangedByUser -= PlaySwipeSound;
            }
        }

        private void Update()
        {
            if (!_isInitialized) return;

            int newIndex = _scrollRectSnap.GetCurrentSnappedIndex();
           // Debug.Log($"Update {newIndex} {_currentIndex}");

            if (newIndex != _currentIndex)
            {
                // Если индекс был валидным, делаем старую точку полупрозрачной
                if (_currentIndex >= 0 && _currentIndex < _visulationCadrIndex.Length)
                {
                    SetAlpha(_visulationCadrIndex[_currentIndex], 0f);
                }
                
                _currentIndex = newIndex;
                
                // Делаем новую точку непрозрачной
                if (_currentIndex >= 0 && _currentIndex < _visulationCadrIndex.Length)
                {
                    SetAlpha(_visulationCadrIndex[_currentIndex], 1f);
                }
            }
        }

        private void PlaySwipeSound()
        {
            AudioManager.Instance.PlaySound(_swipeSFX);
        }

        private void SetupSwipe()
        {
            // Инициализируем скроллер
            _scrollRectSnap.Initialize(_lvls);
            _scrollRectSnap.ResetToStart(); // Сбрасываем на начальный элемент

            _isInitialized = true;
        }
        
        private void SetAlpha(Image image, float alpha)
        {
            if (image == null) return;
            Color color = image.color;
            color.a = alpha;
            image.color = color;
        }
    }
}