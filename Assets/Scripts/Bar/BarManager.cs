using SpaceShooter;
using System.Collections;
using System.Collections.Generic;
using TowerDefense;
using UnityEngine;

namespace QuizCinema
{
	public class BarManager : SingletonBase<BarManager>
	{
		[SerializeField] private MapLevel _level;
		[SerializeField] private GameObject _panelOpenNextBar;
		[SerializeField] private bool _isOpen = false;

		private const string _completeLvlSFX = "CompleteLvl";

		private void Start()
		{
			//ResetProgress();
			var numberBar = int.Parse((_level.Episode.EpisodeName.Substring(_level.Episode.EpisodeName.Length - 1)));

			Debug.Log(CheckBarOpen(numberBar) + " открыт ли следующий бар! " + PlayerPrefs.GetInt(_level.Episode.EpisodeName));
			if (CheckBarOpen(numberBar) && PlayerPrefs.GetInt(_level.Episode.EpisodeName) == 0)
				OnBarOpenInfoUpdate();
			else if (PlayerPrefs.GetInt(_level.Episode.EpisodeName) == 0)
			{
				Debug.Log("test 0");
			}
		}

		public bool CheckBarOpen(int indexEpisode)
		{
			var starsEpisode = MapCompletion.Instance.GetEpisodeStars(indexEpisode);
			var needStarsToOpenBar = StorageBarsInfo.Instance.InfoBars[indexEpisode - 1].NeedStarsScore;
			if (starsEpisode >= needStarsToOpenBar)
				return true;
			return false;
		}

		private void OnBarOpenInfoUpdate()
		{
			StartCoroutine(ActivateNewBarPanel());
			Debug.Log(MapCompletion.Instance.GetOpensBar[_level.Episode.EpisodeID - 1] + " ... " + (_level.Episode.EpisodeID - 1));
			
		}


		private IEnumerator ActivateNewBarPanel()
		{
			yield return new WaitForSeconds(LevelSequenceController.Instance.TimeAnimClick * 2);

			ShowOpenNewBar();
		}

		public void ShowOpenNewBar()
		{
			_panelOpenNextBar.SetActive(true);
			AudioManager.Instance.PlaySound(_completeLvlSFX);

			PlayerPrefs.SetInt(_level.Episode.EpisodeName, 1);
		}


		public void ResetProgress()
		{
			PlayerPrefs.SetInt(_level.Episode.EpisodeName, 0);
		}
	}
}