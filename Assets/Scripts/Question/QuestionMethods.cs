using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static QuizCinema.GameManager;

namespace QuizCinema
{
    public class QuestionMethods : SingletonBase<QuestionMethods>, IDependency<GameManager>
    {
        public event Action<Question> OnUpdateQuestionUI;
        public event Action<List<AnswerData>> CurrentAnswerList;

        [SerializeField] private GameManager _gameManager;

        private Data _data = new Data();
        public Data Data { get { return _data; } set { _data = value; } }

        private List<AnswerData> _pickedAnswers = new List<AnswerData>();
        public List<AnswerData> PickedAnswers => _pickedAnswers;

        private List<int> _finishedQuestions = new List<int>();
        public List<int> FinishedQuestions => _finishedQuestions;
        public int GetFinishedLengthQuestions => _finishedQuestions.Count();

        protected int _currentIndexQuestion = 0;
        public int CurrentIndexQuestion { get { return _currentIndexQuestion; } set { _currentIndexQuestion = value; } }
        public virtual bool IsFinished => IsFinishedCheck();
        public bool IsFinishedClassic;

        public int GetLengthQuestions => _data.Questions.Length;

        public int _currentIndexNotRandom = 0;
        private void OnEnable()
        {
            AnswerData.UpdateQuestionAnswer += UpdateAnswers;
            ConfirmAnswer.OnAcceptAnswer += UpdateAnswers;
        }

        protected virtual bool IsFinishedCheck()
		{
            return _finishedQuestions.Count < _data.Questions.Length ? false : true;
        }

        private void OnDisable()
        {
            AnswerData.UpdateQuestionAnswer -= UpdateAnswers;
            ConfirmAnswer.OnAcceptAnswer -= UpdateAnswers;
        }

        protected virtual void Start()
        {
            _currentIndexNotRandom = 0;
        }

        public void UpdateAnswers(AnswerData newAnswer)
        {
            Debug.Log("UpdateAnswers");
            if (_data.Questions[_currentIndexNotRandom].GetAnswerType == AnswerType.Single)
            {
                foreach (var answer in _pickedAnswers.ToList())
                {
                    if (answer != newAnswer)
                    {
                        answer.Reset();
                    }

                }
                _pickedAnswers.Clear();
                _pickedAnswers.Add(newAnswer);
                CurrentAnswerList?.Invoke(_pickedAnswers);
                _gameManager.Accept();
            }
            else
            {
                Debug.Log("Multiple");
                _pickedAnswers.Add(newAnswer);
                if (_pickedAnswers.Count == 1)
				{
                    Debug.Log("Multiple count = 1");
                    //_gameManager.Accept();
                }
                
                if (_pickedAnswers.Count == _data.Questions[_currentIndexNotRandom].GetCorrectAnswers().Count)
                {
                    CurrentAnswerList?.Invoke(_pickedAnswers);
                    _gameManager.Accept();
                }
            }

            
        }

        public void EraseAnswers()
        {
            _pickedAnswers = new List<AnswerData>();
        }


        public Question GetRandomQuestion()
        {
            int randomIndex = GetRandomQuestionIndex();
            _currentIndexNotRandom = randomIndex;

            return _data.Questions[_currentIndexNotRandom];
        }

        public Question GetNotRandomQuestion(int index)
        {
            return _data.Questions[index];
        }

        public int GetRandomQuestionIndex()
        {
            var random = 0;
            if (_finishedQuestions.Count < _data.Questions.Length)
            {
                do
                {
                    random = UnityEngine.Random.Range(0, _data.Questions.Length);
                } while (_finishedQuestions.Contains(random) || _currentIndexNotRandom == random);
            }
            return random;
        }

        public bool CheckAnswers()
        {
            if (!CompareAnswers())
            {
                return false;
            }
            return true;
        }


        public bool CompareAnswers()
        {
            if (_pickedAnswers.Count > 0)
            {
                List<int> correctAnswersList = _data.Questions[_currentIndexNotRandom].GetCorrectAnswers();
                List<int> pickedAnswersList = _pickedAnswers.Select(x => x.AnswerIndex).ToList();

                var firstList = correctAnswersList.Except(pickedAnswersList).ToList();
                var secondList = pickedAnswersList.Except(correctAnswersList).ToList();

                Debug.Log(correctAnswersList[0] + " CompareAnswers");

                return !firstList.Any() && !secondList.Any();
            }
            return false;
        }

        public List<int> ReturnInCorrectAnswers(List<AnswerData> currentAnswers)
        {
            List<int> pickedAnswersList = currentAnswers.Select(x => x.AnswerIndex).ToList();
            if (pickedAnswersList.Count > 0)
            {
                List<int> correctAnswersList = _data.Questions[_currentIndexNotRandom].GetCorrectAnswers();
                

                var firstList = correctAnswersList.Except(pickedAnswersList).ToList();
                var secondList = pickedAnswersList.Except(correctAnswersList).ToList();
                return secondList;
            }
            return pickedAnswersList;
        }


        public void Display()
        {
            EraseAnswers();

            // var question = GetRandomQuestion();
            var question = GetNotRandomQuestion(_currentIndexNotRandom);

           // if (_finishedQuestions.Count < _data.Questions.Length)
           //     _currentIndexNotRandom++;

            OnUpdateQuestionUI?.Invoke(question);

        }

        public void Display(bool updateQuestionUI = false)
        {
            Debug.Log(_currentIndexNotRandom + " Display");

            EraseAnswers();
            // var question = GetRandomQuestion();
            var question = GetNotRandomQuestion(_currentIndexNotRandom);

            // if (_finishedQuestions.Count < _data.Questions.Length)
            //     _currentIndexNotRandom++;
            if (updateQuestionUI)
                OnUpdateQuestionUI?.Invoke(question);

        }

        public void Construct(GameManager obj)
        {
            _gameManager = obj;
        }
    }
}