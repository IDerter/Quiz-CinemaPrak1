using SpaceShooter;
using System.Collections;
using TowerDefense;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace QuizCinema
{
	public class UIFinishElementsClassic : UIFinishElements
    {
        [SerializeField] private GameManagerClassic _gameManagerClassic;

        protected override void OnFinishGame()
        {
            Debug.Log("OnFinishGame UIFINISH");
            _retryButton.SetActive(true);

            if (!_gameManagerClassic.IsReborn)
            {
                _nextButton.SetActive(true);
            }
            else
            {
                _nextButton.SetActive(false);
            }

            AnalyticsManager.Instance.SaveClassicStats(
                SceneManager.GetActiveScene().buildIndex,
                _gameManagerClassic.CountCorrectAnswer,
                MapCompletion.Instance.MaxRecordClassic);
        }

        public override void RestartGame()
        {
            _gameManagerClassic.IsReborn = false;
            AnalyticsManager.Instance.RestartLeveStats(SceneManager.GetActiveScene().buildIndex);
            StartCoroutine(DelayRestart(LevelSequenceController.Instance.TimeAnimClick));
        }

        private IEnumerator DelayRestart(float time)
        {
            yield return new WaitForSeconds(time);

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}