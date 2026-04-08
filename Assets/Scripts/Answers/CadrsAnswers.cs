using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QuizCinema
{
    public class CadrsAnswers : AnswerData
    {
        public static event Action<AnswerData, bool> UpdateButtonCadr;

        [SerializeField] private Image _imageOutline;

        [SerializeField] private bool _pressButtonCheck = false;

        private void Start()
        {
          //  _imageOutline.gameObject.SetActive(false);
        }



        private void OnEnable()
        {
            _pressButtonCheck = false;
            GameManager.Instance.OnCorrectAnswer += OnCorrectAnswer;
            GameManager.Instance.OnInCorrectAnswer += OnUnCorrectAnswer;
        }

        private void OnDisable()
        {
            //  _imageOutline.gameObject.SetActive(false);
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
                    CorrectAnswer.SetActive(true);
                    Debug.Log("CorrectAnswer!");
                }
            }

        }

        private void OnUnCorrectAnswer()
        {
            Debug.Log("Start InCorrectAnswer!");


            foreach (var i in QuestionMethods.Instance.PickedAnswers)
            {
                if (i == this)
                {
                    InCorrectAnswer.SetActive(true);
                  
                    Debug.Log("InCorrectAnswer!");
                }
            }
            
        }


        public void FirstTouch()
        {
            _pressButtonCheck = !_pressButtonCheck;
            UpdateButtonCadr?.Invoke(this, _pressButtonCheck);
            Debug.Log(_pressButtonCheck);
         //   _imageOutline.gameObject.SetActive(_pressButtonCheck);
        }

        public override void SwitchCase()
        {
            base.SwitchCase();
        }

    }
}