using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QuizCinema {
    public class BoostFifiyOnFifty : BoostParent
    {
        [SerializeField] private Question _currentQuestion;

        private const int _countRemoveAnswer = 2;

        private const int _indexPrefabImageQuestions = 3;

        protected override void OnCreateAnswers(Question question)
        {
            if (_buttonBoost.TryGetComponent<BoostUICount>(out var boost))
            {
                _boostSO = boost.GetSetBoostSO;
            }

            _currentQuestion = question;

           // SwitchInteractable(true, _buttonBoost);

            if (_currentQuestion.IndexPrefab == 3 || _currentQuestion.GetAnswerType == AnswerType.Multiply)
            {
                _buttonBoost.SetActive(false);
            }
        }

        public override void ActivateBoost(bool everyQuestionActivate)
        {
            //base.ActivateBoost();

            var currentAnswer = AnswersMethods.Instance.GetCurrentAnswerList;

            var listIndexCorrectAnswer = _currentQuestion.GetCorrectAnswers();

            int indexPrefab = _currentQuestion.IndexPrefab;

            int currentAnswerIndex = 0;
            for (int i = 0; i < _currentQuestion.Answers.Length; i++)
            {
                if (_currentQuestion.GetAnswerType == AnswerType.Single)
                {
                    if (listIndexCorrectAnswer[0] != i && currentAnswerIndex < _countRemoveAnswer && indexPrefab != _indexPrefabImageQuestions)
                    {
                        currentAnswer[i].gameObject.SetActive(false);
                        currentAnswerIndex++;
                    }

                    else if (listIndexCorrectAnswer[0] != i && currentAnswerIndex < _countRemoveAnswer)
                    {
                        currentAnswer[i].CurrentImage.enabled = false;
                        currentAnswerIndex++;
                    }
                }

				else
				{
                    if ((listIndexCorrectAnswer[0] != i && listIndexCorrectAnswer[1] != i) && currentAnswerIndex < _countRemoveAnswer && indexPrefab != _indexPrefabImageQuestions)
                    {
                        currentAnswer[i].gameObject.SetActive(false);
                        currentAnswerIndex++;
                    }
                } 

            }

            SwitchInteractable(false, _buttonBoost);

            if (!everyQuestionActivate)
                BoostsManager.UseBoost(_boostSO);
        }
    }
}