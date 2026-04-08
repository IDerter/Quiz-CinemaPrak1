using SpaceShooter;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace QuizCinema
{
    public class LoadingLvlManager : MonoBehaviour
    {
        [SerializeField] protected Sprite[] _lvlLoadingsSprites;
        [SerializeField] protected Image _imageLoadingLvl;

        [SerializeField] protected Image _backgroundImage;
        [SerializeField] protected Image[] _sliderImages;
        [SerializeField] protected Image _skinImage;
        [SerializeField] protected SliderLoadLvl _sliderLoadLvl;

        [SerializeField] protected float _fadeDuration = 1f;

        private void Awake()
        {
            var skinIndex = SkinManager.GetIndexPutOnSkin();
            _imageLoadingLvl.sprite = _lvlLoadingsSprites[skinIndex];
        }

        private void OnEnable()
        {
            _sliderLoadLvl.OnSliderCompleted += FadeOut;
        }

        private void OnDisable()
        {
            _sliderLoadLvl.OnSliderCompleted -= FadeOut;
        }

        private void Start()
        {
            FadeIn();
        }

        public void FadeIn()
        {
            _imageLoadingLvl.raycastTarget = true;
            _backgroundImage.DOFade(1f, _fadeDuration);

            foreach (var img in _sliderImages)
                img.DOFade(1f, _fadeDuration);

            _skinImage.DOFade(0.3f, _fadeDuration);
        }

        public void FadeOut()
        {
            Debug.Log("FadeOut");
            _backgroundImage.DOFade(0f, _fadeDuration);

            foreach (var img in _sliderImages)
                img.DOFade(0f, _fadeDuration);

            _skinImage.DOFade(0f, _fadeDuration);
            _imageLoadingLvl.raycastTarget = false;

        }
    }
}