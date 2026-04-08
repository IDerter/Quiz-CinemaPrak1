using SpaceShooter;
using System.Collections;
using TowerDefense;
using UnityEngine;
using UnityEngine.UI;

namespace QuizCinema
{
	public class LearningLvlManager : SingletonBase<LearningLvlManager>
	{
		[SerializeField] private FillSlider _fillSlider;
		[SerializeField] private GameObject _overlayHintsTimer;
		[SerializeField] private GameObject _overlayHintsFinishScreen;
		[SerializeField] private GameObject _overlayHintsBoosterDescription;
		[SerializeField] private GameObject _overlayHintsBoosterAfterClick;
		[SerializeField] private TimerInLvl _timerInLvl;
		[SerializeField] private Button _buttonPlay;
		[SerializeField] private Button _buttonRetry;
		[SerializeField] private GameObject _handHintSwipe;
		[SerializeField] private GameObject _adventureLearningPanel;
		[SerializeField] private MapLevel _firstLevelAdventure;

		private void Start()
		{
			if (_fillSlider != null)
				_fillSlider.OnEndFillSlider += OnEndFillSlider;

			if (GameManager.Instance != null)
			{
				GameManager.Instance.OnFinishGame += OnFinishGame;
				GameManager.Instance.OnDownloadedQuestions += OnDownloadedQuestions;
			}
		}

		private void OnDestroy()
		{
			if (_fillSlider != null)
				_fillSlider.OnEndFillSlider -= OnEndFillSlider;

			if (GameManager.Instance != null)
			{
				GameManager.Instance.OnFinishGame -= OnFinishGame;
				GameManager.Instance.OnDownloadedQuestions -= OnDownloadedQuestions;
			}
		}

		public void StartLevel()
		{
			// написть логику
			if (!MapCompletion.Instance.LearnSteps[0])
			{
				if (_adventureLearningPanel != null)
				{
					StartCoroutine(DelayOverlaySetActive(_adventureLearningPanel, LevelSequenceController.Instance.TimeAnimClick));
				}
				return;
			}
			if (_firstLevelAdventure != null)
				_firstLevelAdventure.LoadLevel();
		}

		public void FinishLearningAdventure()
		{
			if (!MapCompletion.Instance.LearnSteps[0])
			{
				MapCompletion.Instance.LearnSteps[0] = true;
				MapCompletion.SaveLearningProgress();
				MapCompletion.SaveFinishLearining();
			}
			StartLevel();
		}


		private void OnDownloadedQuestions()
		{
			Debug.Log("OnDownloaded");
			if (MapCompletion.Instance.LearnSteps[3] && !MapCompletion.Instance.LearnSteps[4])
			{
				if (_overlayHintsBoosterDescription != null)
					StartCoroutine(DelayOverlaySetActive(_overlayHintsBoosterDescription, 1f));
			}
		}

		private void OnFinishGame()
		{
			if (!MapCompletion.Instance.LearnSteps[1])
			{
				_buttonPlay.enabled = false;
				_buttonRetry.enabled = false;
				if (_overlayHintsFinishScreen != null)
					StartCoroutine(DelayOverlaySetActive(_overlayHintsFinishScreen, 2f));
				MapCompletion.Instance.LearnSteps[1] = true;
				MapCompletion.SaveLearningProgress();
				MapCompletion.SaveFinishLearining();
			}
		}

		private void OnEndFillSlider()
		{
			if (MapCompletion.Instance.LearnSteps[1] == false)
			{
				if (_overlayHintsTimer != null)
				{
					StartCoroutine(DelayOverlaySetActive(_overlayHintsTimer, 0.5f));
					_handHintSwipe.SetActive(true);
				}
			}
		}

		private IEnumerator DelayOverlaySetActive(GameObject overlayHints, float time)
		{
			yield return new WaitForSeconds(time);

			overlayHints.SetActive(true);
		}

		public void StartTimer(GameObject overlay)
		{
			StartCoroutine(DelayOverlayTimer(overlay, 0.5f));
		}

		private IEnumerator DelayOverlayTimer(GameObject overlayHints, float time)
		{
			yield return new WaitForSeconds(time);

			overlayHints.SetActive(false);
			_timerInLvl.StartTimer();
			_timerInLvl.IsStopTime = false;
		}

		public void SetOverlayOff(GameObject overlayHints)
		{
			StartCoroutine(DelayOverlayOff(overlayHints, 0.5f));
		}

		private IEnumerator DelayOverlayOff(GameObject overlayHints, float time)
		{
			yield return new WaitForSeconds(time);

			overlayHints.SetActive(false);
		}

		public void ShowGoodJobAfterClickBooster()
		{
			_overlayHintsBoosterDescription.SetActive(false);
			_overlayHintsBoosterAfterClick.SetActive(true);
			MapCompletion.Instance.LearnSteps[4] = true;
			MapCompletion.SaveLearningProgress();
			MapCompletion.SaveFinishLearining();
		}
	}
}