using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TowerDefense;
using UnityEngine;
using static QuizCinema.GameManager;

namespace QuizCinema
{
    public class QuestionMethodsClassic : QuestionMethods
    {

        protected override void Awake()
        {
            base.Awake();
            MapCompletion.OnModesLevelStatsUpdate += OnClassicLevelStatsUpdate;
        }

		protected override void Start()
		{
            _currentIndexNotRandom = MapCompletion.Instance.LastClassicIndex; 
        }

		private void OnDestroy()
        {
            MapCompletion.OnModesLevelStatsUpdate -= OnClassicLevelStatsUpdate;
        }

        private void OnClassicLevelStatsUpdate()
		{
            _currentIndexNotRandom = MapCompletion.Instance.LastClassicIndex;
            Debug.Log($"OnClassicLevelStatsUpdate {_currentIndexNotRandom}");
        }

        protected override bool IsFinishedCheck()
        {
            return IsFinishedClassic;
        }
    }
}