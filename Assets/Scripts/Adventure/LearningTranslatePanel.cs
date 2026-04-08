using QuizCinema;
using UnityEngine;
using UnityEngine.UI;

public class LearningTranslatePanel : MonoBehaviour
{
    [SerializeField] private Sprite[] _skinsLearning;
    [SerializeField] private Sprite[] _scoreResultLearning;
	[SerializeField] private Image _skinImage;
	[SerializeField] private Image _scoreResultImage;

	private void Start()
	{
		OnChangeLangugage();
		if (LocaleSelector.Instance != null)
			LocaleSelector.Instance.OnChangeLangugage += OnChangeLangugage;
	}

	private void OnDestroy()
	{
		if (LocaleSelector.Instance != null)
			LocaleSelector.Instance.OnChangeLangugage -= OnChangeLangugage;
	}

	private void OnChangeLangugage()
	{
		var localeId = LocaleSelector.Instance.GetLocale();
		_skinImage.sprite = _skinsLearning[localeId];
		_scoreResultImage.sprite = _scoreResultLearning[localeId];
	}
}
