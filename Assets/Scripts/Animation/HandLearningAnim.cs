using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace QuizCinema
{
    public class HandLearningAnim : MonoBehaviour
    {
        [SerializeField] private RectTransform _objAnim;

        [SerializeField] private bool _directionLeft = true;
        [SerializeField] private bool _isLoop = true;
        [SerializeField] private float _moveValue = 0.25f;
        [SerializeField] private bool _isDelete = false;
        [SerializeField] private FadeImage _fadeImage;

        private Vector3 _startPos;
        private Sequence _animSequence;

        [SerializeField] private CanvasGroup _handCanvasGroup;

        private void Start()
        {
            if (_objAnim != null)
            {
                // Запоминаем изначальную позицию руки на старте
                _startPos = _objAnim.position;
            }

            if (_isLoop)
                ClickAnim();

            SwipeDetection.OnSwipeInput += OnSwipeInput;
        }

        private void OnDestroy()
        {
            SwipeDetection.OnSwipeInput -= OnSwipeInput;

            // Обязательно убиваем анимацию при удалении объекта, чтобы избежать ошибок
            _animSequence?.Kill();
        }

        private void OnSwipeInput(Vector2 arg0)
        {
            if (_isDelete)
                _fadeImage?.FadeOutStartAnim();
        }

        [Button()]
        public virtual void ClickAnim()
        {
            if (_objAnim == null || _handCanvasGroup == null) return;

            _animSequence?.Kill();

            Vector3 rotate = _objAnim.transform.eulerAngles;
            rotate.z = _directionLeft ? -90 : 90;
            _objAnim.transform.rotation = Quaternion.Euler(rotate);

            float startX = _directionLeft ? _startPos.x + _moveValue : _startPos.x - _moveValue;
            float endX = _directionLeft ? _startPos.x - _moveValue : _startPos.x + _moveValue;

            _animSequence = DOTween.Sequence();

            // 1. Ставим в старт и делаем полностью прозрачной
            _animSequence.Append(_objAnim.DOMoveX(startX, 0f));
            _animSequence.Join(_handCanvasGroup.DOFade(0f, 0f));

            // 2. Плавное появление и нажатие
            _animSequence.Append(_handCanvasGroup.DOFade(1f, 0.2f));
            _animSequence.Join(_objAnim.DOScale(new Vector3(0.95f, 0.95f, 0.95f), 0.2f));

            // 3. Свайп
            _animSequence.Append(_objAnim.DOMoveX(endX, 1f).SetEase(Ease.InOutSine));

            // 4. Отпускаем палец и плавно исчезаем
            _animSequence.Append(_objAnim.DOScale(Vector3.one, 0.2f));
            _animSequence.Join(_handCanvasGroup.DOFade(0f, 0.2f));

            // 5. Небольшая пауза перед новым циклом
            _animSequence.AppendInterval(0.2f);

            if (_isLoop)
            {
                _animSequence.SetLoops(-1, LoopType.Restart);
            }
        }
    }
}