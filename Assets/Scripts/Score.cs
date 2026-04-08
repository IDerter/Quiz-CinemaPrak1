using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuizCinema
{
    public class Score : SingletonBase<Score>
    {
        public event Action UpdateScore;

        [SerializeField] private GameManager _gameManager;

        [Header("Score")]

        [SerializeField] private int _currentLvlScore;
        public int CurrentLvlScore { get { return _currentLvlScore; } set { _currentLvlScore = value; }}


        protected override void Awake()
        {
            base.Awake();
            _currentLvlScore = 0;

        }

        public void UpdateScoreGame(int add)
        {
            _currentLvlScore += add;
            if (_currentLvlScore <= 0)
                _currentLvlScore = 0;
            Debug.Log(add + " Сколько добавили к счету!");

            UpdateScore?.Invoke();
        }

    }
}