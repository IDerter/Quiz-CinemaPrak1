using SpaceShooter;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TowerDefense;
using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

namespace QuizCinema
{
    public class GameManagerClassic : GameManager
    {
        [SerializeField] private int _startIndex = 0;
        [SerializeField] private int _endIndex = 0;
        [SerializeField] private GameObject _gameFinishElements;
        [SerializeField] public bool IsReborn { get; set; }
        [SerializeField] private LoadingLvlManagerClassic _loadingLvlManager;
        [SerializeField] private string _leaderBoardClassicName = "BestPlayersClassic";

        private int _countSkipAdd = 5;

        [Header("Оптимизация трафика S3")]
        [Tooltip("Сколько картинок держать скачанными впереди текущего прогресса игрока")]
        [SerializeField] private int _preloadBufferSize = 5;

        protected override void Start()
        {
            base.Start();

            if (YG2.platform == "YandexGames")
            {
                YG2.onRewardAdv += RebornRewarded;
                YG2.onCloseRewardedAdv += OnYandexAdClosed;
                YG2.onErrorRewardedAdv += OnYandexAdError;
            }
            else
            {
                RewardedAds.RewardOn += RebornRewarded;
            }

            CalculateRangeDownload(10, true);
        }

        private void CalculateRangeDownload(int countDownload, bool isStart = false)
        {
            _startIndex = MapCompletion.Instance.LastClassicIndex;

            if (_questionMethods.Data != null && _startIndex >= _questionMethods.Data.Questions.Length && !isStart)
            {
                _startIndex = 0;
            }

            _startIndex = isStart ? _startIndex : _startIndex + _countSkipAdd;
            _endIndex = _startIndex + countDownload;
        }

        private void ShiftPreloadBuffer(int currentIndex)
        {
            if (_questionMethods.Data == null) return;

            int targetPreloadIndex = currentIndex + _preloadBufferSize;

            if (targetPreloadIndex < _questionMethods.Data.Questions.Length && targetPreloadIndex >= 10)
            {
                string imageToLoad = _questionMethods.Data.Questions[targetPreloadIndex]._cadrCinemaName;

                if (!string.IsNullOrEmpty(imageToLoad) && !BackgroundDownloader.Instance.IsAssetCached(imageToLoad))
                {
                    Debug.Log($"[GameManagerClassic] Докачиваем в фон: {imageToLoad} (Индекс: {targetPreloadIndex})");
                    BackgroundDownloader.Instance.EnqueueForDownload(new[] { imageToLoad }, false);
                }
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (YG2.platform == "YandexGames")
            {
                YG2.onRewardAdv -= RebornRewarded;
                YG2.onCloseRewardedAdv -= OnYandexAdClosed;
                YG2.onErrorRewardedAdv -= OnYandexAdError;
            }
            else
            {
                RewardedAds.RewardOn -= RebornRewarded;
            }
        }

        private void OnYandexAdClosed()
        {
            if (RewardedAds.Instance != null)
                RewardedAds.Instance.OnRewardedAdClosed();
        }

        private void OnYandexAdError()
        {
            Debug.LogError("[GameManager] Yandex ad error");
            if (RewardedAds.Instance != null)
                RewardedAds.Instance.OnRewardedAdClosed();
        }

        public void RebornRewarded(string type)
        {
            if (type == TypeReward.RestartClassicGame.ToString())
            {
                IsReborn = true;
                QuestionMethodsClassic.Instance.IsFinishedClassic = false;

                CalculateRangeDownload(5, true);
                StartCoroutine(Downloader(true));
            }
        }

        protected IEnumerator Downloader(bool isDisplayQuestion)
        {
            _gameFinishElements.SetActive(false);
            Debug.Log("Downloader classic");
            string path = "";

            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                path = "file://" + Application.streamingAssetsPath + "/Q1Classic.xml";
            }
            else
            {
                path = Path.Combine(Application.streamingAssetsPath, "Q1Classic.xml");
            }

            www = new WWW(path);
            yield return www;

            if (www.isDone == true)
            {
                string persistentPath = Path.Combine(Application.persistentDataPath, $"Q{SceneManager.GetActiveScene().buildIndex}Classic.xml");
                File.WriteAllBytes(persistentPath, www.bytes);
                _questionMethods.Data = Data.Fetch(persistentPath);
            }

            if (_questionMethods.Data != null)
            {
                int bgStart = Mathf.Max(10, _startIndex);
                int totalQuestions = _questionMethods.Data.Questions.Length;
                int bgEnd = Mathf.Min(bgStart + _preloadBufferSize, totalQuestions);

                var initialBatch = new List<string>();
                for (int i = bgStart; i < bgEnd; i++)
                {
                    string assetName = _questionMethods.Data.Questions[i]._cadrCinemaName;
                    if (!string.IsNullOrEmpty(assetName)) initialBatch.Add(assetName);
                }

                if (initialBatch.Count > 0)
                {
                    BackgroundDownloader.Instance.EnqueueForDownload(initialBatch, false);
                }

                if (_startIndex >= 10)
                {
                    string currentImg = _questionMethods.Data.Questions[_startIndex]._cadrCinemaName;
                    if (!string.IsNullOrEmpty(currentImg) && !BackgroundDownloader.Instance.IsAssetCached(currentImg))
                    {
                        BackgroundDownloader.Instance.EnqueueForDownload(new[] { currentImg }, true);
                        yield return new WaitUntil(() => BackgroundDownloader.Instance.IsAssetCached(currentImg));
                    }
                }
            }

            yield return new WaitForSeconds(_timeLoadLvl);

            if (isDisplayQuestion)
            {
                _sliderLoadLevel.StartSliderLoadLvl();
                _timerAnimator.enabled = true;

                if (!MapCompletion.Instance.CompleteLearning)
                    _timerInLvl.IsStopTime = true;

                if (_timerInLvl.IsStopTime)
                    _timerInLvl.StopSlider();

                _panelInfoQuiz.SetActive(true);
                _timerInLvl.gameObject.SetActive(true);
                _timerInLvl.IsStopTime = false;
                _timerInLvl.StartTimer();
                _numberQuestionContainer.SetActive(true);

                OnQuestionsDownloaded();

                if (_questionMethods.Data != null)
                {
                    if (_startIndex >= _questionMethods.Data.Questions.Length) _startIndex = 0;
                    _questionMethods._currentIndexNotRandom = _startIndex;

                    _questionMethods.Display(true);
                    _gameFinishElements.SetActive(false);
                }
            }

            if (isDisplayQuestion && _timerInLvl != null && !_timerInLvl.IsStopTime && !_timerInLvl.IsSliderRunning)
            {
                yield return null;
                _timerInLvl.StartTimer();
            }
        }

        protected override IEnumerator Downloader()
        {
            _gameFinishElements.SetActive(false);
            string path = "";

            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                path = "file://" + Application.streamingAssetsPath + "/Q1Classic.xml";
            }
            else
            {
                path = Path.Combine(Application.streamingAssetsPath, "Q1Classic.xml");
            }

            www = new WWW(path);
            yield return www;

            if (www.isDone == true)
            {
                string persistentPath = Path.Combine(Application.persistentDataPath, $"Q{SceneManager.GetActiveScene().buildIndex}Classic.xml");
                File.WriteAllBytes(persistentPath, www.bytes);
                _questionMethods.Data = Data.Fetch(persistentPath);
            }

            if (_questionMethods.Data != null)
            {
                int bgStart = Mathf.Max(10, _startIndex);
                int totalQuestions = _questionMethods.Data.Questions.Length;
                int bgEnd = Mathf.Min(bgStart + _preloadBufferSize, totalQuestions);

                var initialBatch = new List<string>();
                for (int i = bgStart; i < bgEnd; i++)
                {
                    string assetName = _questionMethods.Data.Questions[i]._cadrCinemaName;
                    if (!string.IsNullOrEmpty(assetName)) initialBatch.Add(assetName);
                }

                if (initialBatch.Count > 0)
                {
                    BackgroundDownloader.Instance.EnqueueForDownload(initialBatch, false);
                }

                if (_startIndex >= 10)
                {
                    string currentImg = _questionMethods.Data.Questions[_startIndex]._cadrCinemaName;
                    if (!string.IsNullOrEmpty(currentImg) && !BackgroundDownloader.Instance.IsAssetCached(currentImg))
                    {
                        BackgroundDownloader.Instance.EnqueueForDownload(new[] { currentImg }, true);
                        yield return new WaitUntil(() => BackgroundDownloader.Instance.IsAssetCached(currentImg));
                    }
                }
            }

            yield return new WaitForSeconds(_timeLoadLvl);
            _sliderLoadLevel.StartSliderLoadLvl();
            _timerAnimator.enabled = true;

            if (!MapCompletion.Instance.CompleteLearning)
                _timerInLvl.IsStopTime = true;

            if (_timerInLvl.IsStopTime)
                _timerInLvl.StopSlider();

            _panelInfoQuiz.SetActive(true);
            _timerInLvl.gameObject.SetActive(true);
            _numberQuestionContainer.SetActive(true);

            OnQuestionsDownloaded();

            if (www.isDone == true && _questionMethods.Data != null)
            {
                if (_startIndex >= _questionMethods.Data.Questions.Length) _startIndex = 0;
                _questionMethods._currentIndexNotRandom = _startIndex;
                _questionMethods.Display(true);
            }

            if (_timerInLvl != null && !_timerInLvl.IsStopTime && !_timerInLvl.IsSliderRunning)
            {
                yield return null;
                _timerInLvl.StartTimer();
            }
        }

        protected override void TimerOnEndFillSlider()
        {
            if (MapCompletion.Instance.LastClassicIndex >= _questionMethods.GetLengthQuestions)
            {
                MapCompletion.Instance.LastClassicIndex = 0;
            }
            else
                MapCompletion.Instance.LastClassicIndex += 1;

            IncorrectAnswerLogic();
        }

        public override void Accept()
        {
            if (!_pressButtonAnswer)
            {
                _isCorrectAnswer = _questionMethods.CheckAnswers();
                _countAnswers++;

                if (_timerInLvl != null)
                {
                    _timerInLvl.StopSlider();
                }

                if (MapCompletion.Instance.LastClassicIndex >= _questionMethods.GetLengthQuestions)
                {
                    MapCompletion.Instance.LastClassicIndex = 0;
                }
                else
                    MapCompletion.Instance.LastClassicIndex += 1;

                if (_isCorrectAnswer)
                {
                    _countCorrectAnswer++;
                    _questionMethods.FinishedQuestions.Add(_questionMethods._currentIndexNotRandom);

                    ShiftPreloadBuffer(_questionMethods._currentIndexNotRandom);

                    AfterAnswerСounted();
                }
                else
                {
                    IncorrectAnswerLogic();
                    return;
                }
            }
            _pressButtonAnswer = true;
            NextQuestion();
        }

        private void IncorrectAnswerLogic()
        {
            AudioManager.Instance.PlaySound(_inCorrectSFX);

            _questionMethods.FinishedQuestions.Add(_questionMethods._currentIndexNotRandom);
            MapCompletion.SaveLastIndexClassic();

            if (_countCorrectAnswer > MapCompletion.Instance.MaxRecordClassic)
            {
                MapCompletion.Instance.MaxRecordClassic = _countCorrectAnswer;
                MapCompletion.SaveMaxRecordClassic();
                YG2.SetLeaderboard(_leaderBoardClassicName, _countCorrectAnswer);
            }

            if (_timerInLvl != null)
            {
                _timerInLvl.IsStopTime = true;
                _timerInLvl.StopSlider();
            }

            FinishGameInvoke();
            InCorrectAnswerInvoke();
            FinishGame();
        }

        protected override void AfterAnswerСounted()
        {
            _countAnswers = 0;

            if (IE_WaitTillNextRound != null)
            {
                StopCoroutine(IE_WaitTillNextRound);
            }

            var type = (_isCorrectAnswer) ? UIManager.ResolutionScreenType.Correct : UIManager.ResolutionScreenType.Incorrect;
            OnUpdateDisplayScreenResolution(type);
        }

        protected override void FinishGame()
        {
            AdsManager.Instance._interstitialAds.LoadInterstitialAd();
            AdsManager.Instance._rewardedAds.LoadRewardedAd();

            var type = UIManager.ResolutionScreenType.Finish;
            OnUpdateDisplayScreenResolution(type);

            QuestionMethodsClassic.Instance.IsFinishedClassic = true;
            _loadingLvlManager.FadeIn();
        }

        protected override IEnumerator WaitTillNextRound()
        {
            yield return new WaitForSeconds(GameUtility.ResolutionDelayTime);

            if (_timerInLvl != null)
            {
                _timerInLvl.StopSlider();
                _timerInLvl.IsStopTime = false; // Снимаем таймер с паузы, но пока не запускаем
            }

            if (_questionMethods != null && _questionMethods.Data != null)
            {
                int nextIndex = _questionMethods._currentIndexNotRandom;

                if ((nextIndex + 1) >= MapCompletion.Instance.LastClassicIndex && MapCompletion.Instance.LastClassicIndex == _questionMethods.Data.Questions.Length)
                {
                    nextIndex = 0;
                }
                else
                {
                    nextIndex++;
                }

                // 1. СНАЧАЛА ПЕРЕХОДИМ НА СЛЕДУЮЩИЙ ВОПРОС
                // UIManager сам установит _loadingSprite, пока картинка качается
                _questionMethods._currentIndexNotRandom = nextIndex;
                _questionMethods.Display();

                // 2. ЖДЕМ ЗАГРУЗКИ (ЕСЛИ ИЗОБРАЖЕНИЕ ИЗ S3 И ЕЩЕ НЕ В КЭШЕ)
                if (nextIndex >= 10)
                {
                    string nextImageName = _questionMethods.Data.Questions[nextIndex]._cadrCinemaName;

                    if (!string.IsNullOrEmpty(nextImageName) && !BackgroundDownloader.Instance.IsAssetCached(nextImageName))
                    {
                        Debug.LogWarning($"[GameManagerClassic] Картинка {nextImageName} еще качается. Приостанавливаем таймер...");
                        BackgroundDownloader.Instance.EnqueueForDownload(new[] { nextImageName }, true);

                        float waitTime = 0f;
                        float maxWaitTime = 15f; // Лимит ожидания 15 секунд

                        while (!BackgroundDownloader.Instance.IsAssetCached(nextImageName) && waitTime < maxWaitTime)
                        {
                            yield return new WaitForSeconds(0.5f);
                            waitTime += 0.5f;
                        }

                        // Если прошло 15 секунд, а картинки так и нет - выдаем критическую ошибку сети
                        if (!BackgroundDownloader.Instance.IsAssetCached(nextImageName))
                        {
                            Debug.LogError($"[GameManagerClassic] Время ожидания вышло! Картинка {nextImageName} не загрузилась.");
                            BackgroundDownloader.Instance.ReportError("Слабое интернет соединение. Не удалось загрузить вопрос.");

                            // Прерываем корутину: таймер не запустится, игра встанет на паузу (будет висеть панель ошибки)
                            yield break;
                        }
                    }
                }
            }

            // 3. ЗАПУСКАЕМ ТАЙМЕР (только когда мы уверены, что картинка готова)
            if (_timerInLvl != null)
            {
                yield return null; // Даем 1 кадр на синхронизацию UI
                if (!_timerInLvl.IsSliderRunning)
                {
                    _timerInLvl.InitiliazeSlider();
                    _timerInLvl.StartTimer();
                }
            }
        }
    }
}