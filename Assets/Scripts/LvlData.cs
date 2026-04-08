using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace QuizCinema
{
    [Serializable]
    public class LvlData : MonoBehaviour
    {
        [Header("Lvl")]
        private int _level = 1;
        private const int _maxLevel = 2;
        public int MaxLevel => _maxLevel;

        public int Level { get { return _level; } set { _level = value; } }

        private void Awake()
        {
            _level = PlayerPrefs.GetInt(GameUtility.SavePrefLvlKey, 1);
            Debug.Log(_level);
        }
    }
}