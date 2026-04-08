using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace QuizCinema
{
    public class BoostSlot : BoostUICount
    {
        [SerializeField] private Sprite _defaultUnchooseSpriteBoost;
        public Sprite DefaultSprite { get { return _defaultUnchooseSpriteBoost; } set { _defaultUnchooseSpriteBoost = value; } }
        [SerializeField] private Image _warningImage;
        public Image WarningImage => _warningImage;

    }
}