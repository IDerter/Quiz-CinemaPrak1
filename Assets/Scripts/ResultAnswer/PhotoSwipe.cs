using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QuizCinema
{
    public class PhotoSwipe : MonoBehaviour
    {
        [SerializeField] private Image _photoHolderCorrect;
        [SerializeField] private Image _photoHolderInCorrect;

        private Question _currentQuestion;

        private void Start()
        {
            AnswersMethods.Instance.OnCreateAnswers += OnCreateAnswers;
            AnswerData.UpdateQuestionAnswer += UpdateQuestionAnswer;
        }


        private void OnDestroy()
        {
            AnswersMethods.Instance.OnCreateAnswers -= OnCreateAnswers;
            AnswerData.UpdateQuestionAnswer -= UpdateQuestionAnswer;
        }

        private void OnCreateAnswers(Question question)
        {
            Debug.Log("OnCreateAnswers");
            _currentQuestion = question;

        }


        private void UpdateQuestionAnswer(AnswerData answerData)
        {
            if (isActiveAndEnabled)
            {
                _photoHolderCorrect.sprite = answerData.CurrentImage.sprite;
                _photoHolderInCorrect.sprite = answerData.CurrentImage.sprite;
            }
        }
    }
}