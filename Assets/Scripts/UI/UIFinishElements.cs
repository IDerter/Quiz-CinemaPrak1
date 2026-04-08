using Cysharp.Threading.Tasks;
using SpaceShooter;
using System;
using System.Collections;
using TowerDefense;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YG;

namespace QuizCinema
{
	public class UIFinishElements : MonoBehaviour
    {
        [SerializeField] protected GameManager _gameManager;
        [SerializeField] protected GameObject _nextButton;
        [SerializeField] protected GameObject _retryButton;

        private bool _isAdInProgress = false;

        private void Start()
        {
            _gameManager.OnFinishGame += OnFinishGame;
			UIManager.Instance.OnFinishScoreCalculating += OnFinishScoreCalculating;
        }

		protected virtual void OnFinishScoreCalculating()
		{
            _retryButton.SetActive(true);

            if (_gameManager.GetLevelCountStars >= 1)
            {
                _nextButton.SetActive(true);
            }
            else
            {
                _nextButton.SetActive(false);
            }
        }

		private void OnDestroy()
        {
            _gameManager.OnFinishGame -= OnFinishGame;
            UIManager.Instance.OnFinishScoreCalculating -= OnFinishScoreCalculating;
            InterstitialAds.OnInterstitialAdClosed -= HandleAdClosed;
        }

        protected virtual void OnFinishGame()
        {
            Debug.Log("OnFinishGame UIFINISH");
            AnalyticsManager.Instance.SaveLevelStarsStats(SceneManager.GetActiveScene().buildIndex, _gameManager.GetLevelCountStars);
            Debug.Log(_gameManager.GetLevelCountStars);
        }

        public virtual void RestartGame()
        {
            AnalyticsManager.Instance.RestartLeveStats(SceneManager.GetActiveScene().buildIndex);
;
            StartCoroutine(DelayRestart(LevelSequenceController.Instance.TimeAnimClick));
        }

        private IEnumerator DelayRestart(float time)
        {
            yield return new WaitForSeconds(time);

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public async void NextLvl()
        {
            if (_isAdInProgress) return;

            Debug.Log("next lvl");
            if (MapCompletion.Instance.CompleteLearning)
            {
                if (MapCompletion.Instance.CountLvlFinished % 2 == 0 && YG2.platform != "YandexGames")
                {
                    _isAdInProgress = true;
                    var nextButtonComponent = _nextButton.GetComponent<Button>();
                    nextButtonComponent.interactable = false;

                    // Запускаем защитный таймер
                    var resetTask = ResetButtonAfterDelay(10f);

                    try
                    {
                        var adShown = new UniTaskCompletionSource();
                        Action onAdClosed = () => adShown.TrySetResult();
                        InterstitialAds.OnInterstitialAdClosed += onAdClosed;

                        AdsManager.Instance._interstitialAds.ShowInterstitialAd();

                        // Ждем рекламу или таймаут
                        await UniTask.WhenAny(
                            adShown.Task,
                            UniTask.Delay(TimeSpan.FromSeconds(10), ignoreTimeScale: true)
                        );

                        InterstitialAds.OnInterstitialAdClosed -= onAdClosed;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[UIFinishElements] Error showing ad: {e.Message}");
                    }
                    finally
                    {
                        // В любом случае разблокируем
                        _isAdInProgress = false;
                        nextButtonComponent.interactable = true;
                    }
                }
                else if (YG2.platform == "YandexGames")
				{
                    _isAdInProgress = true;
                    var nextButtonComponent = _nextButton.GetComponent<Button>();
                    nextButtonComponent.interactable = false;

                    // Запускаем защитный таймер
                    var resetTask = ResetButtonAfterDelay(10f);

                    try
                    {
                        var adShown = new UniTaskCompletionSource();
                        Action onAdClosed = () => adShown.TrySetResult();
                        InterstitialAds.OnInterstitialAdClosed += onAdClosed;

                        AdsManager.Instance._interstitialAds.ShowInterstitialAd();

                        // Ждем рекламу или таймаут
                        await UniTask.WhenAny(
                            adShown.Task,
                            UniTask.Delay(TimeSpan.FromSeconds(10), ignoreTimeScale: true)
                        );

                        InterstitialAds.OnInterstitialAdClosed -= onAdClosed;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[UIFinishElements] Error showing ad: {e.Message}");
                    }
                    finally
                    {
                        // В любом случае разблокируем
                        _isAdInProgress = false;
                        nextButtonComponent.interactable = true;
                    }
                }

                await LoadSceneAsync();
            }
            else
            {
                await LoadExitClassicLevelMenuAsync();
            }
        }

        // Убираем ResetButtonAfterDelay, так как логика теперь в основном методе

        private async UniTask ResetButtonAfterDelay(float delay)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay), ignoreTimeScale: true);

            // Принудительный сброс, если что-то пошло не так
            if (_isAdInProgress)
            {
                Debug.LogWarning("[UIFinishElements] Force resetting button state after timeout");
                _isAdInProgress = false;
                _nextButton.GetComponent<Button>().interactable = true;
            }
        }

        private async UniTask LoadExitClassicLevelMenuAsync()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(LevelSequenceController.Instance.TimeAnimClick));
            LevelSequenceController.Instance.ExitToClassicLevelMenu();
        }


        private async UniTask LoadSceneAsync()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(LevelSequenceController.Instance.TimeAnimClick));
            LevelSequenceController.Instance.NextLevel();
        }

        private void HandleAdClosed()
        {
            // Этот метод может быть пустым или содержать доп. логику,
            // но он нужен для отписки от события
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}