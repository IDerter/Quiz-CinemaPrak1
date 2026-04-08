using System.Collections;
using TowerDefense;
using UnityEngine;
using UnityEngine.UI;

namespace QuizCinema
{
    public class LearningMapManager : MonoBehaviour
    {
		[SerializeField] private GameObject _overlayFirstMapOpenHint;
		[SerializeField] private GameObject _bar1;
		[SerializeField] private Button _bar1Button;
		[SerializeField] private GameObject _overlaySettings;
		[SerializeField] private VolumeValue _volumeSettings;

		private void Start()
		{
			Debug.Log(MapCompletion.Instance.LearnSteps[0]);
			if (MapCompletion.Instance.LearnSteps[0] == true)
			{
				_bar1Button.interactable = true;
				StartCoroutine(DelayHintActive(false));
			}
			else
			{
				_bar1Button.interactable = false;
				StartCoroutine(DelayHintActive(true));
			}
		}

		public IEnumerator DelayHintActive(bool isActivate)
		{
			yield return new WaitForSeconds(1f);
			if (isActivate)
				_volumeSettings.SetSliderValue();

			_overlayFirstMapOpenHint.SetActive(isActivate);
			_bar1.SetActive(!isActivate);
			_overlaySettings.SetActive(isActivate);
			MapCompletion.Instance.LearnSteps[0] = true;
			MapCompletion.SaveLearningProgress();
		}
	}
}