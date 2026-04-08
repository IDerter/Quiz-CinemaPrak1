using SpaceShooter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using TowerDefense;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using YG;
using Cysharp.Threading.Tasks;

namespace QuizCinema
{
    public class GameManager : SingletonBase<GameManager>, IDependency<QuestionMethods>, IDependency<Score>
    {
        public event Action<UIManager.ResolutionScreenType, int> UpdateDisplayScreenResolution;
        public event Action OnFinishGame;
        public event Action OnDownloadedQuestions;
 
        public event Action OnCorrectAnswer;
        public event Action OnInCorrectAnswer;
 
        public event Action OnNextQuestion;
 
        #region Variables
        [SerializeField] protected TimerInLvl _timerInLvl;
        [SerializeField] protected QuestionMethods _questionMethods;
        [SerializeField] protected Score _score;
        [SerializeField] protected Animator _timerAnimator;
        [SerializeField] protected GameObject _numberQuestionContainer;
        [SerializeField] protected GameObject _panelInfoQuiz;
        [SerializeField] protected Animator _loadLvlBgAnimator;
        [SerializeField] protected float _timeLoadLvl = 2f;
        public float GetTimeLoadLvl => _timeLoadLvl;
        public Animator TimerAnimator { get { return _timerAnimator; } set { value = _timerAnimator; } }

        protected bool _isCorrectAnswer;
        protected int _levelCountStars = 3;
        public int GetLevelCountStars => _levelCountStars;

        protected const string _correctSFX = "CorrectSFX";
        protected const string _inCorrectSFX = "IncorrectSFX";
        protected const string _inCorrectTimeEndSFX = "TimerEnd";
        protected const string _winLvlSFX = "WinLvl";

        [Header("Lvl")]
        protected int _loadingScreenStateParaHash = 0;
        protected IEnumerator IE_WaitTillNextRound = null;
        protected WWW www;
        protected bool _pressButtonAnswer = false;

        [SerializeField] protected int _countCorrectAnswer = 0;
        public int CountCorrectAnswer { get { return _countCorrectAnswer; } set { _countCorrectAnswer = value; } }
        [SerializeField] protected int _countCurrentAnswer = 1;
        public int CountCurrentAnswer { get { return _countCurrentAnswer; } set { _countCurrentAnswer = value; } }
        #endregion

        protected bool _isActivateBoost50Percent = false;
        public bool IsActivateBoost50Percent { get { return _isActivateBoost50Percent; } set { _isActivateBoost50Percent = value; } }
        [SerializeField] private bool _isRewarded;
        public bool IsRewarded { get { return _isRewarded; } set { _isRewarded = value; } }
        protected int _countAnswers = 0;

        [Header("Notifications")]
        [SerializeField] protected GameObject _networkErrorPanel; // Ссылка на панель с уведомлением об ошибке сети
        [SerializeField] protected SliderLoadLvl _sliderLoadLevel;

        [SerializeField] private string _leaderBoardAdventureName = "BestPlayersAdventure";

        public void Construct(QuestionMethods obj)
        {
            _questionMethods = obj;
        }

        public void Construct(Score obj)
        {
            _score = obj;
        }

        protected virtual void Start()
        {
            _loadingScreenStateParaHash = Animator.StringToHash("Loading Screen");
            _timerInLvl.OnEndFillSlider += TimerOnEndFillSlider;

            if (YG2.platform == "YandexGames")
            {
                YG2.onRewardAdv += RewardOn;
                YG2.onCloseRewardedAdv += OnYandexAdClosed;
                YG2.onErrorRewardedAdv += OnYandexAdError;
            }
            else
            {
                RewardedAds.RewardOn += RewardOn;
            }

            StartCoroutine(Downloader());

            var seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            UnityEngine.Random.InitState(seed);
        }

        protected virtual void OnDestroy()
        {
            if (YG2.platform == "YandexGames")
            {
                YG2.onRewardAdv -= RewardOn;
                YG2.onCloseRewardedAdv -= OnYandexAdClosed;
                YG2.onErrorRewardedAdv -= OnYandexAdError;
            }
            else
            {
                RewardedAds.RewardOn -= RewardOn;
            }
        }

        private void OnYandexAdClosed()
        {
            if (RewardedAds.Instance != null)
            {
                RewardedAds.Instance.OnRewardedAdClosed();
            }
        }

        private void OnYandexAdError()
        {
            Debug.LogError("[GameManager] Yandex ad error");

            if (RewardedAds.Instance != null)
            {
                RewardedAds.Instance.OnRewardedAdClosed();
            }
        }

        private void RewardOn()
        {
            _score.UpdateScoreGame(_score.CurrentLvlScore / 2);
            _levelCountStars = CalculateLevelStars();
            UIManager.Instance.StartCalculateScore(_levelCountStars);
        }

        private void RewardOn(string type)
        {
            if (type == TypeReward.X1_5.ToString())
            {
                _score.UpdateScoreGame(_score.CurrentLvlScore / 2);
                _levelCountStars = CalculateLevelStars();
                UIManager.Instance.StartCalculateScore(_levelCountStars);
            }
        }

        protected virtual void TimerOnEndFillSlider()
        {
            _questionMethods.FinishedQuestions.Add(_questionMethods._currentIndexNotRandom);
            var numberBar = LevelSequenceController.Instance.CurrentEpisode.EpisodeID;
            var numberLvlInBar = LevelSequenceController.Instance.CurrentLevel;
            var scoreAdd = StorageBarsInfo.Instance.InfoBars[numberBar - 1].ScoreLvlsInBar[numberLvlInBar % 5] / _questionMethods.GetLengthQuestions;
            _score.UpdateScoreGame(-scoreAdd);
            AudioManager.Instance.PlaySound(_inCorrectTimeEndSFX);
            NextQuestion();
        }

        protected virtual void OnQuestionsDownloaded()
        {
            OnDownloadedQuestions?.Invoke();
        }

        protected virtual void OnUpdateDisplayScreenResolution(UIManager.ResolutionScreenType type)
        {
            UpdateDisplayScreenResolution?.Invoke(type, _questionMethods.Data.Questions[_questionMethods._currentIndexNotRandom].AddScore);
        }

        protected virtual void InCorrectAnswerInvoke()
        {
            OnInCorrectAnswer?.Invoke();
        }

        protected virtual void FinishGameInvoke()
        {
            Debug.Log("FinishGameInvoke");
            OnFinishGame?.Invoke();
        }

        protected virtual IEnumerator Downloader()
        {
            string path = "";
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                path = "file://" + Application.streamingAssetsPath + $"/Q{SceneManager.GetActiveScene().buildIndex}.xml";
            }
            else if (Application.platform == RuntimePlatform.WebGLPlayer || Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                path = Path.Combine(Application.streamingAssetsPath, $"Q{SceneManager.GetActiveScene().buildIndex}.xml");
            }
            
            www = new WWW(path);
            yield return www;

           string levelKey = $"Q{SceneManager.GetActiveScene().buildIndex}";

           // Ставим ассеты текущего уровня в приоритетную очередь
           BackgroundDownloader.Instance.EnqueueForPriorityDownload(levelKey);

           // Ждем, пока все ассеты для этого уровня не будут готовы, с тайм-аутом
           float waitTime = 0f;
           float maxWaitTime = 10f; // Максимальное время ожидания 10 секунд

           while (!BackgroundDownloader.Instance.AreAssetsReadyForLevel(levelKey))
           {
               Debug.Log($"[GameManager] Waiting for assets for level {levelKey} to be downloaded...");

               if (waitTime >= maxWaitTime)
               {
                   string error = $"Timeout waiting for assets for level {levelKey}.";
                   Debug.LogError($"[GameManager] {error}");
                   BackgroundDownloader.Instance.ReportError(error);
                   yield return new WaitForSeconds(1f); // Даем время на отображение сообщения
                   if (!BackgroundDownloader.Instance.IsMapDataLoaded)
                   {
                       _networkErrorPanel.SetActive(true);
                   }
                    yield break; // Выходим из корутины
               }

               if (Application.internetReachability == NetworkReachability.NotReachable)
               {
                   string error = $"No internet connection, and assets for {levelKey} are not cached.";
                   Debug.LogError($"[GameManager] {error}");
                   BackgroundDownloader.Instance.ReportError(error);
                   yield return new WaitForSeconds(1f);
                   //SceneManager.LoadScene("Map"); // Укажите имя вашей главной сцены или сцены с картой
                   yield break;
               }

               yield return new WaitForSeconds(0.5f);
               waitTime += 0.5f;
           }

           Debug.Log($"[GameManager] All assets for {levelKey} are ready. Starting level.");

            yield return new WaitForSeconds(_timeLoadLvl); // Это больше не нужно, ожидание происходит выше
            //  _loadLvlBgAnimator.enabled = true;
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

            if (www.isDone == true)
            {
                string persistentPath = Path.Combine(Application.persistentDataPath, $"Q{SceneManager.GetActiveScene().buildIndex}.xml");
                File.WriteAllBytes(persistentPath, www.bytes);
                _questionMethods.Data = Data.Fetch(persistentPath);
                
                _questionMethods.Display();

                var nextLevelKey = $"Q{SceneManager.GetActiveScene().buildIndex + 1}";

                // Ставим ассеты след уровня в приоритетную очередь
                BackgroundDownloader.Instance.EnqueueForPriorityDownload(nextLevelKey);
            }
        }

        public virtual void Accept()
        {
            if (!_pressButtonAnswer)
            {
                _isCorrectAnswer = _questionMethods.CheckAnswers();
                _countAnswers++;
                _timerInLvl.StopSlider();
                if (_isCorrectAnswer)
                {
                    OnCorrectAnswer?.Invoke();
                    _countCorrectAnswer++;
                }
                else
                {
                    OnInCorrectAnswer?.Invoke();
                }
                _questionMethods.FinishedQuestions.Add(_questionMethods._currentIndexNotRandom);

                var numberBar = LevelSequenceController.Instance.CurrentEpisode.EpisodeID;
                var numberLvlInBar = LevelSequenceController.Instance.CurrentLevel;
                var scoreAdd = StorageBarsInfo.Instance.InfoBars[numberBar - 1].ScoreLvlsInBar[numberLvlInBar % 5] / _questionMethods.GetLengthQuestions;
                _score.UpdateScoreGame(_isCorrectAnswer ? scoreAdd : -scoreAdd);
                AudioManager.Instance.PlaySound((_isCorrectAnswer) ? _correctSFX : _inCorrectSFX);
                Invoke("AfterAnswerСounted", 0.5f);
            }
            _pressButtonAnswer = true;
        }

        protected virtual void AfterAnswerСounted()
        {
            _countAnswers = 0;
            var numberBar = LevelSequenceController.Instance.CurrentEpisode.EpisodeID;
            var numberLvlInBar = LevelSequenceController.Instance.CurrentLevel;
            var scoreAdd = StorageBarsInfo.Instance.InfoBars[numberBar - 1].ScoreLvlsInBar[numberLvlInBar % 5] / _questionMethods.GetLengthQuestions;

            if (IE_WaitTillNextRound != null)
            {
                StopCoroutine(IE_WaitTillNextRound);
            }

            var type = (_isCorrectAnswer) ? UIManager.ResolutionScreenType.Correct : UIManager.ResolutionScreenType.Incorrect;
            UpdateDisplayScreenResolution?.Invoke(type, scoreAdd);
        }

        public virtual void NextQuestion()
        {
            if (_questionMethods.GetLengthQuestions > _countCurrentAnswer)
                _countCurrentAnswer++;

            Debug.Log("NextQuestion");
            if (!_questionMethods.IsFinished)
            {
                OnNextQuestion?.Invoke();
                IE_WaitTillNextRound = WaitTillNextRound();
                StartCoroutine(IE_WaitTillNextRound);

                Debug.Log("OnNextQuestion");
            }
            else
            {
                Debug.Log("NextQuestion IsFinished = true");

                AudioManager.Instance.StopSound("BGLvl");
                _levelCountStars = CalculateLevelStars();
                if (_levelCountStars > 0)
                {
                    MapCompletion.Instance.CountLvlFinished++;
                    MapCompletion.SaveLvlFinished();
                    AudioManager.Instance.PlaySound(_winLvlSFX);

                    var currentIndex = MapCompletion.Instance.GetLastAdventureLevelIndex();
                    Debug.Log($"Current Index {currentIndex}");

                    if (SceneManager.GetActiveScene().buildIndex >= currentIndex)
                        YG2.SetLeaderboard(_leaderBoardAdventureName, SceneManager.GetActiveScene().buildIndex);
                }
                // Сначала вызываем событие, чтобы бустеры успели применить свои эффекты
                OnFinishGame?.Invoke();
                // И только потом завершаем игру и сохраняем результат с уже обновленными очками
                FinishGame();
            }
            _pressButtonAnswer = false;
        }

        protected virtual void FinishGame()
        {
            if (_questionMethods.IsFinished)
            {
                AdsManager.Instance._interstitialAds.LoadInterstitialAd();
                AdsManager.Instance._rewardedAds.LoadRewardedAd();

                UIManager.Instance.StartCalculateScore(_levelCountStars);

                var type = UIManager.ResolutionScreenType.Finish;
                OnUpdateDisplayScreenResolution(type);
            }
        }

        public int CalculateLevelStars()
        {
            if (_countCorrectAnswer >= (_countCurrentAnswer - 1))
            {
                _levelCountStars = 3;
            }
            else if (_countCorrectAnswer >= Math.Ceiling(_countCurrentAnswer / 1.5))
            {
                _levelCountStars = 2;
            }
            else if (_countCorrectAnswer >= Math.Ceiling(_countCurrentAnswer / 2.0))
            {
                _levelCountStars = 1;
            }
            else
            {
                _levelCountStars = 0;
            }
            return _levelCountStars;
		}

		protected virtual IEnumerator WaitTillNextRound()
		{
			yield return new WaitForSeconds(GameUtility.ResolutionDelayTime);
			_timerInLvl.StopSlider();
			_questionMethods._currentIndexNotRandom++;
			_questionMethods.Display();
			_timerInLvl.StartSlider();

            Debug.Log("WaitTillNextRound");
		}
	}
}
