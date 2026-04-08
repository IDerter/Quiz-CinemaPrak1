using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TowerDefense;
using SpaceShooter;
using TMPro;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.Networking;
using System.IO;

namespace QuizCinema
{
    public class UIManager : SingletonBase<UIManager>
    {
        public event Action OnFinishScoreCalculating;
        public enum ResolutionScreenType { Correct, Incorrect, Finish }

        [Header("References")]
        [SerializeField] protected GameManager _gameManager;
        [SerializeField] protected QuestionMethods _questionMethods;
        [SerializeField] protected Score _score;

        [SerializeField] protected SettingUIManager _settingUIManager;

        [Header("UI Elements (Prefabs)")]
        [SerializeField] protected AnswerData[] _answerPrefab;
        [SerializeField] protected RectTransform _panelInfoQuiz;

        [SerializeField] protected UIElements _uIElements;
        [SerializeField] protected ScrollRectSnap _scrollRectSnap;

        [Header("Notifications")]
        [SerializeField] protected GameObject _networkErrorPanel; // Ссылка на панель с уведомлением об ошибке сети

        [Header("Image Loading Placeholders")]
        [SerializeField] protected Sprite _loadingSprite;
        [SerializeField] protected Sprite _errorSprite;

        [SerializeField] protected bool _isCalculating;
        public bool IsCalculating { get { return _isCalculating; } set { _isCalculating = value; } }

        protected List<int> _finishedAnswers = new List<int>();
        public List<int> FinishedAnswers => _finishedAnswers;

        protected List<AnswerData> _currentAnswer = new List<AnswerData>();
        public List<AnswerData> GetCurrentAnswerList { get { return _currentAnswer; } set { _currentAnswer = value; } }

        protected List<AnswerData> _correctAnswer = new List<AnswerData>();
        public List<AnswerData> GetCorrectAnswerList { get { return _correctAnswer; } set { _correctAnswer = value; } }

        protected int _resStateParaHash = 0;

        protected IEnumerator IE_DisplayTimedResolution;

        protected ResolutionScreenType _typeAnswer;

        private void OnEnable()
        {
            _questionMethods.OnUpdateQuestionUI += UpdateQuestionUI;
            _gameManager.UpdateDisplayScreenResolution += DisplayResolution;
            _gameManager.OnFinishGame += OnFinishGame;
            _score.UpdateScore += UpdateScoreUI;

            if (BackgroundDownloader.Instance != null)
            {
                BackgroundDownloader.Instance.OnDownloadError += HandleDownloadError;
            }
        }

        private void OnFinishGame()
        {
            Debug.Log("OnFinishGame");
            UpdateFinishScreen();
        }

        private void OnDisable()
        {
            _questionMethods.OnUpdateQuestionUI -= UpdateQuestionUI;
            _gameManager.UpdateDisplayScreenResolution -= DisplayResolution;
            _gameManager.OnFinishGame -= OnFinishGame;
            _score.UpdateScore -= UpdateScoreUI;

            if (BackgroundDownloader.Instance != null)
            {
                BackgroundDownloader.Instance.OnDownloadError -= HandleDownloadError;
            }
        }

        private void Start()
        {
            _resStateParaHash = Animator.StringToHash("ScreenState");
            _answerPrefab = _settingUIManager.AnswersPrefabs;
        }

        public void SetPanelInfoValues(bool panelUp)
        {
            var valueY = panelUp ? 1 : 0;
            _panelInfoQuiz.anchorMax = new Vector2(0.5f, valueY);
            _panelInfoQuiz.anchorMin = new Vector2(0.5f, valueY);
            _panelInfoQuiz.pivot = new Vector2(0.5f, valueY);
        }

        public virtual void UpdateQuestionUI(Question question)
        {
            Debug.Log("UpdateQuestionUI");
            UpdateScoreUI();

            var index = question.IndexPrefab;
            if (index <= 1)
            {
                SetPanelInfoValues(false);
            }
            else if (index == 2 || index == 3)
            {
                SetPanelInfoValues(true);
            }

            for (int i = 0; i < _answerPrefab.Length; i++)
            {
                _uIElements.QuestionInfoTextObject[i].transform.parent.gameObject.SetActive(false);
                _uIElements.CadrCinema[i].transform.parent.gameObject.SetActive(false);
                _uIElements.AnswersHolder[i].transform.gameObject.SetActive(false);
            }

            ActivateUIObjects(index, question);

            if (!_questionMethods.IsFinished)
            {
                AnswersMethods.Instance.CreateAnswers(question);
                Debug.Log("CreateAnswers");
            }
            else
            {
                UpdateFinishScreen();
            }
        }

        protected virtual void ActivateUIObjects(int index, Question question)
        {
            _uIElements.QuestionInfoTextObject[index].transform.parent.gameObject.SetActive(true);
            _uIElements.CadrCinema[index].transform.parent.gameObject.SetActive(true);
            _uIElements.AnswersHolder[index].transform.gameObject.SetActive(true);

            _uIElements.QuestionInfoTextObject[index].text = question.ListInfoQuestion[PlayerPrefs.GetInt("IndexLanguageSave")];

            var imageName = question._cadrCinemaName;

           if (string.IsNullOrEmpty(imageName)) return;

            // ГИБРИДНАЯ ЗАГРУЗКА
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            if (buildIndex <= 2 || (SceneManager.GetActiveScene().name == "ClassicRegime" && MapCompletion.Instance.LastClassicIndex >=0 && MapCompletion.Instance.LastClassicIndex <= 9)) // Предполагая, что сцены Q1 и Q2 имеют build index 1 и 2
            {
                Debug.Log("Грузим из Resources");
                // Грузим из Resources
                var spriteFromResources = Resources.Load<Sprite>($"{imageName}");
                if (spriteFromResources != null)
                {
                    _uIElements.CadrCinema[index].sprite = spriteFromResources;
                }
                else
                {
                    _uIElements.CadrCinema[index].sprite = _errorSprite;
                   // Debug.LogError($"[UIManager] Sprite '{imageName}' not found in Resources for level {buildIndex}");
                }
            }
            else
            {
                if (question.IndexPrefab != 2)
                {
                    // Грузим через BackgroundDownloader
                    LoadSpriteAsync(imageName, _uIElements.CadrCinema[index]);
                }
            }
        }

        private async void LoadSpriteAsync(string imageName, Image targetImage)
        {
            if (targetImage == null) return;

            Debug.Log($"[UIManager] ASYNC: Loading '{imageName}'. Setting placeholder.");
            targetImage.sprite = _loadingSprite;

            try
            {
                var sprite = await BackgroundDownloader.Instance.GetSpriteAsync(imageName);

                if (sprite != null && targetImage != null)
                {
                    Debug.Log($"[UIManager] ASYNC: Successfully loaded '{imageName}'. Setting sprite.");
                    targetImage.sprite = sprite;
                }
                else if (targetImage != null)
                {
                    Debug.LogError($"[UIManager] ASYNC: Failed to load sprite for asset name: '{imageName}'. Setting error sprite.");
                    targetImage.sprite = _errorSprite;

                    // Пробуем загрузить из Resources как fallback
                    var fallbackSprite = Resources.Load<Sprite>(imageName);
                    if (fallbackSprite != null)
                    {
                        Debug.Log($"[UIManager] Fallback: Loaded '{imageName}' from Resources");
                        targetImage.sprite = fallbackSprite;
                    }
                    else
					{
                        HandleDownloadError("Ошибка загрузки спрайта на сцене");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[UIManager] Exception loading {imageName}: {e.Message}");
                if (targetImage != null)
                    targetImage.sprite = _errorSprite;
            }
        }

        protected virtual void DisplayResolution(ResolutionScreenType type, int score)
        {
            Debug.Log($"DisplayResolution {type.ToString()}");

            UpdateResUI(type, score);
            _uIElements.ResolutionScreenAnimator.SetInteger(_resStateParaHash, 2);
            _typeAnswer = type;
        }

        public virtual void CloseResultQuestion()
        {
            if (_typeAnswer != ResolutionScreenType.Finish)
            {
                if (IE_DisplayTimedResolution != null)
                {
                    StopCoroutine(IE_DisplayTimedResolution);
                }
                IE_DisplayTimedResolution = DisplayTimedResolution();
                StartCoroutine(IE_DisplayTimedResolution);
            }

            if (_typeAnswer == ResolutionScreenType.Finish)
            {
                _uIElements.FinishUIElements.gameObject.SetActive(true);
            }
        }

        protected virtual IEnumerator DisplayTimedResolution()
        {
            yield return new WaitForSeconds(GameUtility.ResolutionDelayTime);
            Debug.Log("DisplayTimedResolution");
            _uIElements.ResolutionScreenAnimator.SetInteger(_resStateParaHash, 1);
        }

        protected virtual void UpdateResUI(ResolutionScreenType type, int score)
        {
            var currentEpisode = LevelSequenceController.Instance.CurrentEpisode;
            var sceneName = SceneManager.GetActiveScene().name;
            _uIElements.NumberCurrentQuestion.text = _gameManager.CountCurrentAnswer + "/" + _questionMethods.Data.Questions.Length;
        }

        protected virtual void UpdateFinishScreen()
        {
            Debug.Log("UpdateFinishScreen");
            var sceneName = SceneManager.GetActiveScene().name;

            if (_uIElements.CountCorrectAnswer.Length == 2)
            {
                _uIElements.CountCorrectAnswer[0].text = _gameManager.CountCorrectAnswer + "/10";
                _uIElements.CountCorrectAnswer[1].text = _gameManager.CountCorrectAnswer + "/10";
            }
            else
            {
                Debug.LogError("Fill in the field CountCorrectAnswer");
            }
        }
        
        public virtual void StartCalculateScore(int levelCountsStars)
        {
            Debug.Log("StartCalculateScore " + MapCompletion.Instance.LearnSteps[1]);
            CalculateScore(levelCountsStars);
        }

        public void FinishScoreCalculating()
		{
            OnFinishScoreCalculating?.Invoke();
        }

        public virtual void CalculateScore(int levelCountsStars)
        {
            Debug.Log("StartCalculate with DOTween");

            if (_score.CurrentLvlScore == 0)
            {
                if (_uIElements.ScoreFinalLvl.Length == 2)
                {
                    _uIElements.ScoreFinalLvl[0].text = "0";
                    _uIElements.ScoreFinalLvl[1].text = "0";
                }
            }

            MapCompletion.SaveEpisodeResult(levelCountsStars, _score.CurrentLvlScore);

            var maxScoreCurrentLvl = MapCompletion.Instance.GetLvlMaxScore(SceneManager.GetActiveScene().name);
            bool isNewMaxScore = _score.CurrentLvlScore > maxScoreCurrentLvl;

            if (!isNewMaxScore && _uIElements.MaxScoreFinalLvl.Length == 2)
            {
                _uIElements.MaxScoreFinalLvl[0].text = maxScoreCurrentLvl.ToString();
                _uIElements.MaxScoreFinalLvl[1].text = maxScoreCurrentLvl.ToString();
            }

            _isCalculating = true;

            int startScore = 0;
            int targetScore = _score.CurrentLvlScore;
            float animationDuration = 1.5f;

            DOTween.To(() => startScore, x => startScore = x, targetScore, animationDuration)
                .SetEase(Ease.OutQuad)
                .OnUpdate(() =>
                {
                    if (_uIElements.ScoreFinalLvl.Length == 2)
                    {
                        _uIElements.ScoreFinalLvl[0].text = startScore.ToString();
                        _uIElements.ScoreFinalLvl[1].text = startScore.ToString();
                    }
                    if (isNewMaxScore && _uIElements.MaxScoreFinalLvl.Length == 2)
                    {
                        _uIElements.MaxScoreFinalLvl[0].text = startScore.ToString();
                        _uIElements.MaxScoreFinalLvl[1].text = startScore.ToString();
                    }
                })
                .OnComplete(() =>
                {
                    _uIElements.ScoreFinalLvl[0].text = targetScore.ToString();
                    _uIElements.ScoreFinalLvl[1].text = targetScore.ToString();
                    if (isNewMaxScore)
                    {
                        _uIElements.MaxScoreFinalLvl[0].text = targetScore.ToString();
                        _uIElements.MaxScoreFinalLvl[1].text = targetScore.ToString();
                    }

                    OnFinishScoreCalculating?.Invoke();
                    _isCalculating = false;
                    Debug.Log("Score calculation finished.");
                });
        }

        protected virtual void UpdateScoreUI()
        {
            if (_gameManager.CountCorrectAnswer <= _questionMethods.Data.Questions.Length)
                _uIElements.NumberCurrentQuestion.text = _gameManager.CountCurrentAnswer + "/" + _questionMethods.Data.Questions.Length;

            if (_uIElements.ScoreFinalLvl.Length == 2)
            {
                _uIElements.ScoreFinalLvl[0].text = _score.CurrentLvlScore.ToString();
                _uIElements.ScoreFinalLvl[1].text = _score.CurrentLvlScore.ToString();
            }
        }

        private void HandleDownloadError(string errorMessage)
        {
            Debug.LogError($"[UIManager] Network Error: {errorMessage}");
            string currentSceneName = SceneManager.GetActiveScene().name;

            if (currentSceneName == "ClassicRegime")
                MapCompletion.SaveLastIndexClassic();

            if (_networkErrorPanel != null && !(currentSceneName == "Lvl1_1" || currentSceneName == "Lvl1_2"))
            {
                AnalyticsManager.Instance.SaveErrorInLvl(SceneManager.GetActiveScene().buildIndex, errorMessage);
                // Можно добавить логику для отображения текста ошибки на самой панели
                _networkErrorPanel.SetActive(true);
            }
        }
    }
}