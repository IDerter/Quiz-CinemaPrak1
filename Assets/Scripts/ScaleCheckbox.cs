using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuizCinema
{
    public class ScaleCheckbox : MonoBehaviour
    {
        [SerializeField] private RectTransform _answerPrefab;
        [SerializeField] private RectTransform _checkBox;

        private void Update()
        {
            Debug.Log(_answerPrefab.rect.width);

        }
    }
}