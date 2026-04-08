using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QuizCinema
{
    public class MoveSlider : MonoBehaviour
    {
        [SerializeField] private Slider _sliderVolume;
        [SerializeField] private Image _sliderFillVolume;
        public Image SliderFillVolume { get { return _sliderFillVolume; } set { _sliderFillVolume = value; } }

        [SerializeField] private Slider _sliderMusic;
        [SerializeField] private Image _sliderFillMusic;
        public Image SliderFillMusic { get { return _sliderFillMusic; } set { _sliderFillMusic = value; } }

        private void Start()
        {
            _sliderMusic?.onValueChanged.AddListener(delegate { ValueChangeMusic(); });
            _sliderVolume?.onValueChanged.AddListener(delegate { ValueChangeVolume(); });
        }

        private void ValueChangeVolume()
        {
            _sliderFillVolume.fillAmount = _sliderVolume.value;
        }

        private void ValueChangeMusic()
        {
            _sliderFillMusic.fillAmount = _sliderMusic.value;
        }

        public void OpenHelpPage()
		{
            Application.OpenURL("https://kaeruartandgames.com/");
		}
    }
}