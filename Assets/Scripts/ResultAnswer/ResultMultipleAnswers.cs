using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace QuizCinema
{
    public class ResultMultipleAnswers : MonoBehaviour
    {
        [SerializeField] private Sprite _almostCorrectBg;

        [SerializeField] private Image _bgResultPanel;


        [Header("Answers")]
        [SerializeField] private TextMeshProUGUI[] _textQuestionNumberResultAllInCorrect;
        [SerializeField] private TextMeshProUGUI[] _textQuestionNumberInLvl;
        [SerializeField] private TextMeshProUGUI[] _textAnswerInfo;
        [SerializeField] private TextMeshProUGUI[] _textCinemaName;
        [SerializeField] private TextMeshProUGUI[] _textCinemaInfo;
        [SerializeField] private Image[] _posterImage;
        private string _directorName;

        [SerializeField] private Button[] _buttonCorrectNext;

        [SerializeField] private List<AnswerData> _listCorrectAnswers;

        private Question _currentQuestion;
        [SerializeField] private string[] _namesFilms;
        [SerializeField] private string[] _namesDescriptions;
        [SerializeField] private TranslateInfo _translate;
        [SerializeField] private Image[] _likeImages;
        [SerializeField] private bool[] _flagLikes;


        [SerializeField] private List<Answer> _listAnswersCorrect;
        [SerializeField] private List<AnswerData> _listAnswersPicked;
        [SerializeField] private List<int> _correctAnswersIndex;

        [SerializeField] private TextMeshProUGUI _correctAnswer1AllCorrectTextInfo;

        [SerializeField] private TextMeshProUGUI _correctAnswer2AllCorrectTextInfo;


        [SerializeField] private TextMeshProUGUI _correctAnswer1TextInfo;

        [SerializeField] private TextMeshProUGUI _inCorrectAnswer1TextInfo;


        [SerializeField] private TextMeshProUGUI _inCorrectAnswer1TextInfo1AllIncorrect;

        [SerializeField] private TextMeshProUGUI _inCorrectAnswer2TextInfo1AllIncorrect;


        private bool _flagShowPanelResult = false;
        private int _indexLang;
        private int correctIndex = 0;

        private const string _clickSFX = "ClickSFX";

        private void Start()
        {
            _indexLang = PlayerPrefs.GetInt("IndexLanguageSave");

            AnswersMethods.Instance.OnCreateAnswers += OnCreateAnswers;
           // AnswersMethods.Instance.OnCorrectAnswer += OnCorrectAnswer;

            QuestionMethods.Instance.CurrentAnswerList += UpdateCurrentAnswerList;
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
                    _textQuestionNumberResultAllInCorrect[0].text = _textQuestionNumberInLvl[0].text;
                    OnCorrectAnswer();
                }
                else
                {
                    _textQuestionNumberResultAllInCorrect[1].text = _textQuestionNumberInLvl[0].text;
                    _textQuestionNumberResultAllInCorrect[2].text = _textQuestionNumberInLvl[0].text;
                    OnInCorrectAnswer();
                }
            }

            _flagShowPanelResult = true;
        }

        private void OnCorrectAnswer()
        {
            if (_currentQuestion._answerType == AnswerType.Multiply)
            {
                _correctAnswer1AllCorrectTextInfo.text = _translate.TextAnswer[_indexLang] + $" \"{_listAnswersCorrect[0].InfoList[_indexLang]}\" " + _translate.TextCorrect[_indexLang];
                _correctAnswer2AllCorrectTextInfo.text = _translate.TextAnswer[_indexLang] + $" \"{_listAnswersCorrect[1].InfoList[_indexLang]}\" " + _translate.TextCorrect[_indexLang];

                if (_listAnswersCorrect[0].InfoList[0] == _namesFilms[0])
                {
                    _textCinemaName[0].text = _listAnswersCorrect[0].InfoList[_indexLang];
                    _textCinemaName[1].text = _listAnswersCorrect[1].InfoList[_indexLang];
                }
                else
                {
                    _textCinemaName[0].text = _listAnswersCorrect[1].InfoList[_indexLang];
                    _textCinemaName[1].text = _listAnswersCorrect[0].InfoList[_indexLang];
                }
            }

            foreach (var buttonCorrect in _buttonCorrectNext)
            {
                buttonCorrect.enabled = true;
                buttonCorrect.gameObject.SetActive(true);
            }
        }



        private void OnInCorrectAnswer()
        {
            Debug.Log("STARTONINCORRECTANSWER! " + _currentQuestion.IndexPrefab);

            if (_currentQuestion.IndexPrefab < 3)
            {

                var indexPicked = -1;
                int correctIndex = -1;
                if (_currentQuestion._answerType == AnswerType.Multiply)
                {
                    _bgResultPanel.sprite = _almostCorrectBg;
                    for (int i = 0; i < _listAnswersPicked.Count; i++)
                    {
                        Debug.Log(_listAnswersPicked[i].AnswerIndex + " " + _listAnswersPicked.Count);
                        if (_listAnswersPicked[i].AnswerIndex == _correctAnswersIndex[0] || _listAnswersPicked[i].AnswerIndex == _correctAnswersIndex[1])
                        {
                            if (_listAnswersCorrect[0].InfoList[_indexLang] == _listAnswersPicked[i].InfoText.text)
                            {
                                correctIndex = 0;
                            }
                            else if (_listAnswersCorrect[1].InfoList[_indexLang] == _listAnswersPicked[i].InfoText.text)
                            {
                                correctIndex = 1;
                            }
                            Debug.Log(correctIndex + "CORRECTINDEX!");
                            _namesFilms = _currentQuestion._cadrCinemaName.Split("!");
                            _namesDescriptions = _currentQuestion.ListDescriptionFilm[_indexLang].Split("!");
                            for (int j = 0; j < _namesFilms.Length; j++)
                            {
                                if (_namesFilms[j] == _listAnswersCorrect[correctIndex].InfoList[0])
                                {
                                    _textCinemaInfo[2].text = _namesDescriptions[j];
                                    _textCinemaName[2].text = _listAnswersCorrect[correctIndex].InfoList[_indexLang];
                                }
                            }

                            Sprite sprite1 = Resources.Load($"Posters/{_listAnswersCorrect[correctIndex].InfoList[0]}_poster", typeof(Sprite)) as Sprite;

                            _posterImage[2].sprite = sprite1;

                            Debug.Log("CORRECT ALMOST!");
                            _textAnswerInfo[2].text = _translate.TextAnswer[_indexLang] + $" \"{_listAnswersPicked[i].InfoText.text}\" " + _translate.TextCorrect[_indexLang];
                        
                            Debug.Log(_correctAnswer1TextInfo.text);
                            indexPicked = i;
                        }
                        else
                        {
                            if (indexPicked != -1 || correctIndex == -1)
                            {
                                Debug.Log("SETACTIVEINCORRECTALMOST!");
                                _inCorrectAnswer1TextInfo.text = _translate.TextAnswer[_indexLang] + $" \"{_listAnswersPicked[i].InfoText.text}\" " + _translate.TextInCorrect[_indexLang];
                            }
                        }

                    }
                
                        _inCorrectAnswer1TextInfo1AllIncorrect.text = _translate.TextAnswer[_indexLang] + $" \"{_listAnswersPicked[0].InfoText.text}\" " + _translate.TextInCorrect[_indexLang];

                        _inCorrectAnswer2TextInfo1AllIncorrect.text = _translate.TextAnswer[_indexLang] + $" \"{_listAnswersPicked[1].InfoText.text}\" " + _translate.TextInCorrect[_indexLang];
                }



            }
        }

        private void OnDestroy()
        {
            AnswersMethods.Instance.OnCreateAnswers -= OnCreateAnswers;
          //  AnswersMethods.Instance.OnCorrectAnswer -= OnCorrectAnswer;

            QuestionMethods.Instance.CurrentAnswerList -= UpdateCurrentAnswerList;

        }

        private void OnCreateAnswers(Question question)
        {
            _currentQuestion = question;
            _flagShowPanelResult = false;

            _listAnswersCorrect.Clear();
            Debug.Log("OnCreateAnswers");
            _currentQuestion = question;

            _correctAnswersIndex = _currentQuestion.GetCorrectAnswers();

            _listAnswersCorrect = AnswersMethods.Instance.GetCorrectAnswerList(_currentQuestion); 

            if (_currentQuestion._answerType == AnswerType.Multiply)
            {

                _namesFilms = _currentQuestion._cadrCinemaName.Split("!");
                _namesDescriptions = _currentQuestion.ListDescriptionFilm[_indexLang].Split("!");

                if (_currentQuestion.ListDescriptionFilm.Count >= 2)
                {
                    _textCinemaInfo[0].text = _namesDescriptions[0];
                    _textCinemaInfo[1].text = _namesDescriptions[1];
                }

                Sprite sprite1 = Resources.Load($"Posters/{_namesFilms[0]}_poster", typeof(Sprite)) as Sprite;
                Sprite sprite2 = Resources.Load($"Posters/{_namesFilms[1]}_poster", typeof(Sprite)) as Sprite;
                _posterImage[0].sprite = sprite1;
                _posterImage[1].sprite = sprite2;

                // Sprite sprite2 = Resources.Load($"Directors/{_currentQuestion.Director}", typeof(Sprite)) as Sprite;
                Debug.Log(_listAnswersCorrect[0].InfoList[0]);



                _textAnswerInfo[0].text = _translate.TextAnswer[_indexLang] + $" \"{_textCinemaName[0]}\" " + _translate.TextCorrect[_indexLang];
                _textAnswerInfo[1].text = _translate.TextAnswer[_indexLang] + $" \"{_textCinemaName[1]}\" " + _translate.TextCorrect[_indexLang];
            }

            StartCoroutine(SetCorrectQuestionText(question));

        }

        IEnumerator SetCorrectQuestionText(Question question)
        {

            Debug.Log("SetCorrectQuestionTextUntilTimer");
            yield return new WaitForSeconds(GameUtility.ResolutionDelayTime);

            _textQuestionNumberResultAllInCorrect[0].text = _textQuestionNumberInLvl[0].text;
            _textQuestionNumberResultAllInCorrect[1].text = _textQuestionNumberInLvl[0].text;
            _textQuestionNumberResultAllInCorrect[2].text = _textQuestionNumberInLvl[0].text;

        }

        public void PressLikeButton(int index)
        {
            AudioManager.Instance.PlaySound(_clickSFX);

            _flagLikes[index] = !_flagLikes[index];
            _posterImage[index].sprite.name = _posterImage[index].sprite.name.Replace("_poster", "");

            LikeFilm.PressButtonLikeMultiple(_likeImages[index], _currentQuestion, _posterImage[index].sprite.name,_textCinemaInfo[index].text,  _flagLikes[index]);
        }

    }
}