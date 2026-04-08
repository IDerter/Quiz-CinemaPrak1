using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QuizCinema
{
    public class InteractableButton : MonoBehaviour
    {
        [SerializeField] private Image[] _imagesButton;

        private void Start()
        {
            _imagesButton = GetComponentsInChildren<Image>();
        }

        public void InteractableOn()
        {
            if (_imagesButton is not null)
            {
                for (int i = 0; i < _imagesButton.Length; i++)
                {
                    _imagesButton[i].color = new Color(1f, 1f, 1f, 1f);
                }
            }
        }

        public void InteractableOff()
        {
            if (_imagesButton is not null)
            {
                for (int i = 0; i < _imagesButton.Length; i++)
                {
                    //_imagesButton[i].color = Color.black;
                    _imagesButton[i].color = new Color(_imagesButton[i].color.r, _imagesButton[i].color.g, _imagesButton[i].color.b, 0.5f);
                }
            }
        }
    }
}