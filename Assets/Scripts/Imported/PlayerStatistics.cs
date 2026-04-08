using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SpaceShooter
{
    public class PlayerStatistics : SingletonBase<PlayerStatistics>
    {
        [SerializeField] private int _numKills;
        public int NumKills { get; set; }

        [SerializeField] private int _score;
        public int Score { get; set; }

        [SerializeField] private int _time;
        public int Time { get; set; }

        [SerializeField] private int _multiplier;
        public int Multiplier { get; set; }
       

        public void Reset()
        {
            _numKills = 0;
            _score = 0;
            _time = 0;
            _multiplier = 1;
        }
    }
}

