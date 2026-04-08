using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace QuizCinema
{
    public class AnswerData : MonoBehaviour
    {
        public static event Action<AnswerData> UpdateQuestionAnswer;

        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI _infoText;
        public TextMeshProUGUI InfoText => _infoText;


        [Header("Textures")]
        [SerializeField] private GameObject _correctAnswer;
        public GameObject CorrectAnswer { get { return _correctAnswer; } set { _correctAnswer = value; } }
        [SerializeField] private GameObject _inCorrectAnswer;
        public GameObject InCorrectAnswer { get { return _inCorrectAnswer; } set { _inCorrectAnswer = value; } }
        [SerializeField] private Image _currentImage;
        public Image CurrentImage { get { return _currentImage; } set { _currentImage = value; } }


        private RectTransform _rect;
        public RectTransform Rect
        {
            get
            {
                if (_rect == null)
                {
                    _rect = GetComponent<RectTransform>() ?? gameObject.AddComponent<RectTransform>();
                }

                return _rect;
            }
        }

        [SerializeField] private int _answerIndex = -1;
        public int AnswerIndex => _answerIndex;

        private bool _checked = false;
        public bool Checked => _checked;

        [SerializeField] private Question _currentQuestion;

        [SerializeField] private List<int> _correctAnswersIndex;



        private void OnEnable()
        {
            GameManager.Instance.OnCorrectAnswer += OnCorrectAnswer;
            GameManager.Instance.OnInCorrectAnswer += OnUnCorrectAnswer;

            _currentQuestion = AnswersMethods.Instance.GetCurrentQuestion;
        }

        private void OnDisable()
        {
            GameManager.Instance.OnCorrectAnswer -= OnCorrectAnswer;
            GameManager.Instance.OnInCorrectAnswer -= OnUnCorrectAnswer;
        }

        private void OnCorrectAnswer()
        {
            Debug.Log("Start CorrectAnswer!");
            foreach (var i in QuestionMethods.Instance.PickedAnswers)
            {
                if (i == this)
                {
                    _correctAnswer.SetActive(true);
                    Debug.Log("CorrectAnswer!");
                }
            }
           
        }

        private void OnUnCorrectAnswer()
        {
            Debug.Log("Start InCorrectAnswer!");
            Question currentQuestion = AnswersMethods.Instance.GetCurrentQuestion;

            if (currentQuestion._answerType == AnswerType.Multiply)
            {
                for (int i = 0; i < QuestionMethods.Instance.PickedAnswers.Count; i++)
                {
                    if ((QuestionMethods.Instance.PickedAnswers[i].AnswerIndex == currentQuestion.GetCorrectAnswers()[0]) ||
                    QuestionMethods.Instance.PickedAnswers[i].AnswerIndex == currentQuestion.GetCorrectAnswers()[1])
                    {
                        if (QuestionMethods.Instance.PickedAnswers[i].AnswerIndex == AnswerIndex)
                        {
                            CorrectAnswer.SetActive(true);
                            Debug.Log("InCORRECT!" + this.InfoText.text);
                            return;
                        }
                    }

                }
            }
            foreach (var i in QuestionMethods.Instance.PickedAnswers)
            {
                if (i == this)
                {
                    _inCorrectAnswer.SetActive(true);
                    _infoText.color = Color.white;
                    Debug.Log("InCorrectAnswer!" + this.InfoText.text);
                    return;
                }
            }
        }


        public void UpdateData(string info, int index)
        {
            _infoText.text = info;
            _answerIndex = index;
        }

        public void Reset()
        {
            _checked = false;
            UpdateUI(_checked);
        }

        public virtual void SwitchCase()
        {
            _checked = !_checked;
            UpdateUI(_checked);

            UpdateQuestionAnswer?.Invoke(this);
            Debug.Log("Switch case");

            _correctAnswersIndex = _currentQuestion.GetCorrectAnswers();

            for (int i = 0; i < _correctAnswersIndex.Count; i++)
            {
                if (_answerIndex == _correctAnswersIndex[i])
                    OnCorrectAnswer();
            }
            if (_correctAnswer.activeSelf == false)
                OnUnCorrectAnswer();
        }

        public void UpdateUI(bool _checked)
        {

            /*if (_checked)
            {
                _correctAnswer.SetActive(true);
            }
            else
            {
                _inCorrectAnswer.SetActive(true);
                _infoText.color = Color.white;
            }
            */
        }
    }
}