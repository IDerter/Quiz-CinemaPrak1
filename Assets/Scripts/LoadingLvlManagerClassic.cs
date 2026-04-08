namespace QuizCinema
{
	public class LoadingLvlManagerClassic : LoadingLvlManager
    {
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
	}
}