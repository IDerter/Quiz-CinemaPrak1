using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QuizCinema
{
    public class BoostIncreaseOn50PersentScore : BoostParent
    {
        public static event Action OnIncrease50PercentBooster;

        private void Start()
        {
            _gameManager.OnFinishGame += OnFinishGame;

            
        }

        private void OnDestroy()
        {
            _gameManager.OnFinishGame -= OnFinishGame;
        }

        private void OnFinishGame()
        {
            if (_buttonPress)
            {
                var score = Score.Instance.CurrentLvlScore;
                score =  Convert.ToInt32(score * 1.5f);
                Score.Instance.CurrentLvlScore = score;
            }
        }

        protected override void OnCreateAnswers(Question question)
        {
            Debug.Log("BoostIncrease");
            if (_buttonBoost.TryGetComponent<BoostUICount>(out var boost))
            {
                _boostSO = boost.GetSetBoostSO;
            }

        }

        public override void ActivateBoost(bool everyQuestionActivate)
        {
            //base.ActivateBoost();
            Debug.Log("50 PERCENT ACTIVE BOOST!");

            _buttonPress = true;

            _gameManager.IsActivateBoost50Percent = true;
           
            if (!everyQuestionActivate)
                BoostsManager.UseBoost(_boostSO);
        }
    }
}