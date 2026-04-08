using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TowerDefense;
using SpaceShooter;
using TMPro;

namespace QuizCinema
{
	public class BarSliderProgress : MonoBehaviour
	{
		[SerializeField] private Image _fiilLoadingBar;
		[SerializeField] private Episode _currentEpisode;
		[SerializeField] private TextMeshProUGUI _textProgressLayer1;
		[SerializeField] private TextMeshProUGUI _textProgressLayer2;

		private int sumStars;

		private void Start()
		{
			sumStars = MapCompletion.Instance.GetEpisodeStars(_currentEpisode.EpisodeID);
			var needSumToOpenBar = StorageBarsInfo.Instance.InfoBars[_currentEpisode.EpisodeID - 1].NeedSumScore;
			var needStarsToOpenBar = StorageBarsInfo.Instance.InfoBars[_currentEpisode.EpisodeID - 1].NeedStarsScore;

			_fiilLoadingBar.fillAmount = (float)sumStars / needStarsToOpenBar;
			_textProgressLayer1.text = sumStars.ToString() + '/' + $"{needStarsToOpenBar}";
			_textProgressLayer2.text = _textProgressLayer1.text;
		}

		private IEnumerator FillProgressSlider()
		{
			float timeLeft = 0;
			// _timerText.color = _timerDefaultColor;

			while (timeLeft <= 1)
			{
				timeLeft += 0.01f;
				_fiilLoadingBar.fillAmount = timeLeft;


				yield return new WaitForSeconds(0.05f);
			}
		}
	}
}