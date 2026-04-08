using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuizCinema
{
    [CreateAssetMenu]
    public class InfoBarScoreOpen : ScriptableObject
    {
        [SerializeField] private int[] _scoreLvlsInBar;
        public int[] ScoreLvlsInBar { get { return _scoreLvlsInBar; } set { _scoreLvlsInBar = value; } }

        [SerializeField] private int _maxSumScore;
        public int MaxSumScore => _maxSumScore;

        [SerializeField] private int _needSumScore;
        public int NeedSumScore => _needSumScore;
        [SerializeField] private int _needStarsScore;
        public int NeedStarsScore => _needStarsScore;
    }
}