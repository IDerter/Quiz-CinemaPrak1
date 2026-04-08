using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TowerDefense;
using SpaceShooter;
using System;

namespace QuizCinema
{
    public class BoostAddScore : BoostParent
    {
        [SerializeField] private int _addValue = 1000;


        protected override void OnCreateAnswers(Question question)
        {
            Debug.Log("Add Score");
            if (_buttonBoost.TryGetComponent<BoostUICount>(out var boost))
            {
                _boostSO = boost.GetSetBoostSO;
            }
        }

        public override void ActivateBoost(bool everyQuestionActivate)
        {
            //base.ActivateBoost();
            Debug.Log("Add Score ACTIVE BOOST!");

            _buttonPress = true;

            var score = Score.Instance.CurrentLvlScore;
            Score.Instance.CurrentLvlScore = score + _addValue;

            if (!everyQuestionActivate)
                BoostsManager.UseBoost(_boostSO);
        }
    }
}