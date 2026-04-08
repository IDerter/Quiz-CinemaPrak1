using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuizCinema
{
    public class InterfaceBoost
    {
        public interface IBoostInventory
        {
            public void LoadInventory();
            
        }

        public interface IBoostInLvl: IBoostInventory
        {
            public void Activate();
        }

        
    }
}