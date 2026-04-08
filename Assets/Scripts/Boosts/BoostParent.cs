using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QuizCinema
{
    public abstract class BoostParent : MonoBehaviour
    {
        public static event Action OnActivateBoost;

        [SerializeField] protected GameManager _gameManager;
        [SerializeField] protected AnswersMethods _answersMethods;

        [SerializeField] protected GameObject _buttonBoost;

        protected BoostSO _boostSO;

        [SerializeField] protected bool _buttonPress = false;
        public bool ButtonPress { get { return _buttonPress; } set { _buttonPress = value; } }
        [SerializeField] protected bool _isStartActiveBoost = false;
        public bool IsStartActiveBoost { get { return _isStartActiveBoost; } set { _isStartActiveBoost = value; } }

        private void OnEnable()
        {
            _answersMethods.OnCreateAnswers += OnCreateAnswers;
        }

        private void OnDestroy()
        {
            _answersMethods.OnCreateAnswers -= OnCreateAnswers;
        }

        protected virtual void OnCreateAnswers(Question obj) 
        {
        }

        public virtual void ActivateBoost(bool everyQuestionActivate) 
        {
            OnActivateBoost?.Invoke();
        }


        protected virtual void SwitchInteractable(bool value, GameObject buttonGameobject) 
        {
            if (buttonGameobject.TryGetComponent<Button>(out Button button))
            {
                button.interactable = value;
            }
        }
    }
}