using UnityEngine;
using System;

namespace QuizCinema
{
    [Serializable]
    public struct UIManagerParameters
    {
        [Header("Resolution Screen Options")]
        [SerializeField] private Color _correctBackgroundColor;
        public Color CorrectBGColor => _correctBackgroundColor;
        [SerializeField] private Color _inCorrectBackgroundColor;
        public Color InCorrectBGColor => _inCorrectBackgroundColor;
        [SerializeField] private Color _finalBackgroundColor;
        public Color FinalBGColor => _finalBackgroundColor;

    }
}