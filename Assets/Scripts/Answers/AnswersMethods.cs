using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuizCinema
{
    public class AnswersMethods : SingletonBase<AnswersMethods> , IDependency<AnswerData>
    {
        public event Action<Question> OnCreateAnswers;
        public event Action<List<AnswerData>> OnCorrectAnswer;
        public event Action<AnswerData> OnCorrectSpriteActivate;

        private List<AnswerData> _currentAnswer = new List<AnswerData>();
        public List<AnswerData> GetCurrentAnswerList { get { return _currentAnswer; } set { _currentAnswer = value; } }

        private List<AnswerData> _correctAnswerData = new List<AnswerData>();
        public List<AnswerData> GetCorrectAnswerDataList { get { return _correctAnswerData; } set { _correctAnswerData = value; } }


        [Header("UI Elements (Prefabs)")]
        [SerializeField] private AnswerData[] _answerPrefab;
        [SerializeField] private RectTransform[] _answerContentArea;
        public RectTransform[] AnswerContentArea => _answerContentArea;
        [SerializeField] private RectTransform _rectTransformStart;
        private Question _currentQuestion;
        public Question GetCurrentQuestion => _currentQuestion;

        public void Construct(AnswerData obj)
        {
            var index = obj.AnswerIndex;
            _answerPrefab[index] = obj;
        }

        private void Start()
        {
            _rectTransformStart = _answerContentArea[3];
        }

        public void CreateAnswers(Question question)
        {
            _currentQuestion = question;

            _correctAnswerData.Clear();

            EraseAnswers();

            var index = question.IndexPrefab;

            var listIndexCorrectAnswer = question.GetCorrectAnswers();

            UpdateCorrectAnswerList(question);

            OnCorrectAnswer?.Invoke(_correctAnswerData);

            OnCreateAnswers?.Invoke(question);
        }

        public List<Answer> GetInCorrectAnswers(Question question)
        {
            List<Answer> incorrectAnswerList = new List<Answer>();
            for (int i = 0; i < question.Answers.Length; i++)
            {
                if (!question.Answers[i].IsCorrect)
                {
                    incorrectAnswerList.Add(question.Answers[i]);
                }
            }
            return incorrectAnswerList;
        }

        public List<Answer> GetCorrectAnswerList(Question question)
        {
            List<Answer> correctAnswerList = new List<Answer>();
            for (int i = 0; i < question.Answers.Length; i++)
            {
                if (question.Answers[i].IsCorrect)
                {
                    correctAnswerList.Add(question.Answers[i]);
                }
            }
            return correctAnswerList;
        }

        public static void Shuffle<T>(T[] arr)
        {
            System.Random rand = new System.Random();

            for (int i = arr.Length - 1; i >= 1; i--)
            {
                int j = rand.Next(i + 1);

                T tmp = arr[j];
                arr[j] = arr[i];
                arr[i] = tmp;
            }
        }


        private void UpdateCorrectAnswerList(Question question)
        {
            var index = question.IndexPrefab;

            Shuffle(question.Answers);
            var listIndexCorrectAnswer = question.GetCorrectAnswers();

            for (int i = 0; i < question.Answers.Length; i++)
            {
                _answerContentArea[3].position = _rectTransformStart.position;
                _answerContentArea[3].sizeDelta = _rectTransformStart.sizeDelta;

                AnswerData newAnswer = Instantiate(_answerPrefab[index], _answerContentArea[index]);
                newAnswer.UpdateData(question.Answers[i].InfoList[PlayerPrefs.GetInt("IndexLanguageSave")], i);

                Debug.Log(question.Answers[i]);
                _currentAnswer.Add(newAnswer);
                Debug.Log(question._cadrCinemaName);

                if (index == 3 && i == 0)
				{
                    if (newAnswer.TryGetComponent(out ImageQuestionAnim rect))
                    {
                       // newAnswer.GetComponent<ImageQuestionAnim>().IsLoop = true;
                       // newAnswer.GetComponent<ImageQuestionAnim>().ClickAnim();
                    }
                    
                }

                if (question.GetAnswerType == AnswerType.Single)
                {
                    if (listIndexCorrectAnswer[0] == i)
                        _correctAnswerData.Add(newAnswer);
                }
                else if (question.GetAnswerType == AnswerType.Multiply)
                {
                    for (int j = 0; j < listIndexCorrectAnswer.Count; j++)
                    {
                        if (i == listIndexCorrectAnswer[j])
                        {
                            _correctAnswerData.Add(newAnswer);
                        }
                    }
                }
            }
        }

        private void EraseAnswers()
        {
            foreach (var answer in _currentAnswer)
            {
                Destroy(answer.gameObject);
            }
            _currentAnswer.Clear();
        }
    }
}