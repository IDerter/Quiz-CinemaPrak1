using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace QuizCinema
{

    public class UIResultBetweenQuestion : MonoBehaviour
    {
        [SerializeField] private TranslateInfo _translate;

        [SerializeField] private GameManager _gameManager;
        [SerializeField] private TextMeshProUGUI[] _textQuestionNumberResult;
        [SerializeField] private TextMeshProUGUI _textQuestionNumberInLvl;

        [Header("Correct and Incorrect")]
        [SerializeField] private Sprite _correctBg;
        [SerializeField] private Sprite _inCorrectBg;
        [SerializeField] private Sprite _almostCorrectBg;

        [SerializeField] private Image _bgResultPanel;
        [SerializeField] private GameObject _textContainerCorrect;
        [SerializeField] private GameObject _textContainerInCorrect;
        [SerializeField] private Button[] _buttonCorrectNext;

        [SerializeField] private TextMeshProUGUI _correctAnswer1TextInfo;
        [SerializeField] private TextMeshProUGUI _inCorrectAnswer1TextInfo;


        [Header("Panel Add Movie")]
        [SerializeField] private TextMeshProUGUI _textCinemaName;
        [SerializeField] private TextMeshProUGUI _textCinemaInfo;
        [SerializeField] private Image _posterImage;
        private string _directorName;
        [SerializeField] private Question _currentQuestion;

        [SerializeField] LikeFilm _like;

        [SerializeField] private List<AnswerData> _listAnswersPicked;
        [SerializeField] private List<Answer> _listAnswersCorrect;
        [SerializeField] private List<Answer> _listAnswersInCorrect;

        [SerializeField] private List<int> _correctAnswersIndex;

        private int _correctIndex;

        private AnswerData _correctAnswerSingle;
        private AnswerData _currentAnswerSingle;

        [SerializeField] private Image _likeImage;
        public Image LikeImage { get { return _likeImage; } set { _likeImage = value; } }
        [SerializeField] private bool _flagLike;

        private int _limitSymbolInQuestion = 200;

        private bool _flagShowPanelResult = false;
        private int _indexLang;

        private const string _clickSFX = "ClickSFX";



        private void Start()
        {
            _indexLang = PlayerPrefs.GetInt("IndexLanguageSave");

            AnswerData.UpdateQuestionAnswer += UpdateQuestionAnswer;

            AnswersMethods.Instance.OnCreateAnswers += OnCreateAnswers;

            QuestionMethods.Instance.CurrentAnswerList += UpdateCurrentAnswerList;

            if (LanguageChanger.Instance != null)
                LanguageChanger.Instance.OnChangeLanguage += OnChangeLanguage;
        }

        private void OnChangeLanguage()
        {

        }

        private void OnDestroy()
        {
            AnswerData.UpdateQuestionAnswer -= UpdateQuestionAnswer;

            AnswersMethods.Instance.OnCreateAnswers -= OnCreateAnswers;

            QuestionMethods.Instance.CurrentAnswerList -= UpdateCurrentAnswerList;

            if (LanguageChanger.Instance != null)
                LanguageChanger.Instance.OnChangeLanguage -= OnChangeLanguage;
        }

        private void UpdateQuestionAnswer(AnswerData answerData)
        {
        }



        private void OnCorrectAnswer()
        {
            _indexLang = PlayerPrefs.GetInt("IndexLanguageSave");
            CorrectSetActive(true);
            Debug.Log("ONCORRECTANSWERS");
            if (_currentQuestion._answerType == AnswerType.Single && _currentQuestion.IndexPrefab != 3)
            {
                _correctAnswer1TextInfo.text = _translate.TextAnswer[_indexLang] + $" \"{_listAnswersCorrect[0].InfoList[_indexLang]}\" " + _translate.TextCorrect[_indexLang];
            }

        }

        private void CorrectSetActive(bool correct)
        {
            var countCorrectAnswer = 0;
            if (_currentQuestion._answerType == AnswerType.Multiply)
            {
                foreach (var answer in _listAnswersPicked)
                {
                    if (answer.AnswerIndex == _correctAnswersIndex[0] || answer.AnswerIndex == _correctAnswersIndex[1])
                        countCorrectAnswer++;
                }
            }

            _bgResultPanel.sprite = correct ? _correctBg : _inCorrectBg;
            if (_currentQuestion._answerType == AnswerType.Multiply && !correct && countCorrectAnswer == 1)
            {
                _bgResultPanel.sprite = _almostCorrectBg;
            }

            _buttonCorrectNext[0].enabled = true;
            _buttonCorrectNext[0].gameObject.SetActive(true);
            _buttonCorrectNext[1].enabled = true;
            _buttonCorrectNext[1].gameObject.SetActive(true);

            _textContainerCorrect.SetActive(correct);

            _textContainerInCorrect.SetActive(!correct);
        }


        private void OnInCorrectAnswer()
        {
            _indexLang = PlayerPrefs.GetInt("IndexLanguageSave");
            Debug.Log("STARTONINCORRECTANSWER! " + _currentQuestion.IndexPrefab);
            CorrectSetActive(false);
            if (_currentQuestion.IndexPrefab < 3 && _currentQuestion._answerType == AnswerType.Single)
            { 

               _inCorrectAnswer1TextInfo.text = _translate.TextAnswer[_indexLang] + $" \"{_listAnswersPicked[0].InfoText.text}\" " + _translate.TextInCorrect[_indexLang];
            }
        }

        private void UpdateCurrentAnswerList(List<AnswerData> answers)
        {
            _listAnswersPicked = answers;
            if (_flagShowPanelResult == false)
            {
                var _isCorrectAnswer = QuestionMethods.Instance.CheckAnswers();
                Debug.Log("ISCORRECTANSWER  " + _isCorrectAnswer);

                if (_isCorrectAnswer)
                {
                    OnCorrectAnswer();
                }
                else
                {
                    OnInCorrectAnswer();
                }
            }

            _flagShowPanelResult = true;

        }

        private void OnCreateAnswers(Question question)
        {

            _flagShowPanelResult = false;
            _listAnswersCorrect.Clear();
            Debug.Log("OnCreateAnswers");
            _currentQuestion = question;

            _correctAnswersIndex = _currentQuestion.GetCorrectAnswers();

            for (int i = 0; i < _correctAnswersIndex.Count; i++)
            {
                _listAnswersCorrect.Add(question.Answers[_correctAnswersIndex[i]]);
            }

            _listAnswersInCorrect = AnswersMethods.Instance.GetInCorrectAnswers(_currentQuestion);


            StartCoroutine(SetCorrectQuestionText(question));
        }

        IEnumerator SetCorrectQuestionText(Question question)
        {

            Debug.Log("SetCorrectQuestionTextDoTimera");
            yield return new WaitForSeconds(GameUtility.ResolutionDelayTime);
            _textQuestionNumberResult[0].text = _textQuestionNumberInLvl.text;
            _textQuestionNumberResult[1].text = _textQuestionNumberInLvl.text;
            LikeFilm.SetDefaultValue(_likeImage);
            SetAllInfoQuestion(question);
        }

        private void SetAllInfoQuestion(Question question)
        {
            Debug.Log("SetCorrectQuestionText");
            _flagLike = false;
            string questionText = question.ListInfoQuestion[_indexLang];
            string correctText = null;

            var mas = questionText.Split('\n');
            int countChar = 0;

            foreach (var sentence in mas)
            {
                if (countChar + sentence.Length < _limitSymbolInQuestion)
                {
                    countChar += sentence.Length;
                    correctText += sentence;
                }
                else
                {
                    correctText += "...";
                    break;
                }
            }


            if (question.ListDescriptionFilm.Count >=2)
                _textCinemaInfo.text = question.ListDescriptionFilm[_indexLang];

            Sprite sprite = Resources.Load($"Posters/{question._cadrCinemaName}_poster", typeof(Sprite)) as Sprite;
            Sprite sprite2 = Resources.Load($"Directors/{question.Director}", typeof(Sprite)) as Sprite;

            _directorName = question.Director;
            _posterImage.sprite = sprite;

            if (_indexLang == 0)
                _textCinemaName.text = question._cadrCinemaName;
            else
                _textCinemaName.text = question._cadrCinemaNameTranslateRu;

            //_textCinemaName.text = _listAnswersCorrect[0].InfoList[_indexLang];
        }


        public void PressLikeButton()
        {
            AudioManager.Instance.PlaySound(_clickSFX);
            _flagLike = !_flagLike;
            LikeFilm.PressButtonLike(_likeImage, _currentQuestion, _flagLike);

            AnalyticsManager.Instance.SaveLikeMovie(_currentQuestion._cadrCinemaName);
        }

    }
}