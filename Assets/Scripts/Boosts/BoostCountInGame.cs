using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QuizCinema
{
    public class BoostCountInGame : BoostUICount
    {
        [SerializeField] private bool _isPress = false;
        [SerializeField] protected Button _buttonBoost;

        private void Start()
        {
            _buttonBoost = GetComponent<Button>();

            _buttonBoost.interactable = true;
            _buttonBoost.enabled = true;
        }

        protected override void CheckButtonInteractable()
        {
            
        }
    }
}