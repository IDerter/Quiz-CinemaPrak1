using DG.Tweening;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using SpaceShooter;

namespace QuizCinema
{
    public class FadeImage : MonoBehaviour
    {
        [SerializeField] private Image _targetImage; // Ссылка на Image компонент, который нужно анимировать
        [SerializeField] private bool _isStartFadeIn;

		private void Start()
		{
            _targetImage = GetComponent<Image>();
            if (_isStartFadeIn)
                FadeInStartAnim();
		}

		private void OnEnable()
		{
            _targetImage = GetComponent<Image>();
            if (_isStartFadeIn)
                FadeInStartAnim();
        }

		public async void FadeOutStartAnim()
		{
            await FadeOutAsync();
        }

        public async void FadeInStartAnim()
        {
            await FadeInAsync();
        }

        private async UniTask FadeOutAsync()
        {
            if (_targetImage == null)
            {
                Debug.LogError("Target Image is not assigned.");
                return;
            }

            // Использование DoTween для анимации альфа-канала изображения
            await _targetImage.DOFade(0f, LevelSequenceController.Instance.TimeAnimClick).SetEase(Ease.Linear).ToUniTask();

        }

        private async UniTask FadeInAsync()
        {
            if (_targetImage == null)
            {
                Debug.LogError("Target Image is not assigned.");
                return;
            }

            // Использование DoTween для анимации альфа-канала изображения
            await _targetImage.DOFade(1f, LevelSequenceController.Instance.TimeAnimClick).SetEase(Ease.Linear).ToUniTask();
        }
    }
}