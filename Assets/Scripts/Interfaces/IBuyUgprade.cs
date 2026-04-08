using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuizCinema
{
    public interface IBuyUgprade
    {
        public void Initialize();
        public void CheckCost(int money, bool _isBuy = false);
        public void Buy();
    }
}
