using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuizCinema
{
    public class StorageBarsInfo : SingletonBase<StorageBarsInfo>
    {
        [SerializeField] private InfoBarScoreOpen[] _infoBars;
        public InfoBarScoreOpen[] InfoBars => _infoBars;
    }
}