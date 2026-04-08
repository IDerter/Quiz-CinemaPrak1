using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace QuizCinema
{
    public class CopyText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textOriginal;
        [SerializeField] private List<TextMeshProUGUI> _textToCopy;

        private void Start()
        {
            for (int i = 0; i < _textToCopy.Count; i++)
            {
                _textToCopy[i].text = _textOriginal.text;
            }
        }
    }
}