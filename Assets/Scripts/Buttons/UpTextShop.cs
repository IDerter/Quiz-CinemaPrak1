using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuizCinema
{
    public class UpTextShop : MonoBehaviour
    {
        [SerializeField] private GameObject _textsChoose;
        [SerializeField] private GameObject _textsUnchoose;

        public void ActivateText()
		{
            _textsChoose.SetActive(true);
            _textsUnchoose.SetActive(false);
        }

        public void DisableText()
		{
            _textsChoose.SetActive(false);
            _textsUnchoose.SetActive(true);
        }
    }
}