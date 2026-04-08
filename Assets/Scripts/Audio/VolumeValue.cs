using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace QuizCinema
{
    public class VolumeValue : MonoBehaviour
    {
        [SerializeField] private AudioMixerGroup _audioMixerGroup;
        [SerializeField] private AudioMixerGroup _SFXAudioMixer;
        [SerializeField] private AudioMixerGroup _musicAudioMixer;

        [SerializeField] private MoveSlider _slidersSettings;


     //   [SerializeField] private Toggle _toogleMusic;
     //   [SerializeField] private Toggle _toogleSFX;
        [SerializeField] private Slider _silderSoundsVolume;
        [SerializeField] private Slider _silderMusicVolume;

        private float minVolumeValue = -80f;
        private float maxVolumeValue = 0f;

        private const string _clickSFX = "ClickSFX";

        private void Start()
        {
            Debug.Log("Load volume data");

            _audioMixerGroup.audioMixer.SetFloat("MusicVolume", Mathf.Lerp(-40, 0, PlayerPrefs.GetFloat("MusicVolumeSave", 1)));
            _audioMixerGroup.audioMixer.SetFloat("SoundsVolume", Mathf.Lerp(-40, 0,  PlayerPrefs.GetFloat("SoundsVolumeSave", 1)));
        }

        public void GameStop()
        {
            AudioManager.Instance.PlaySound(_clickSFX);
            Time.timeScale = 0;
        }

        public void GameStart()
        {
            Time.timeScale = 1;
            AudioManager.Instance.PlaySound(_clickSFX);
        }

        public void SetSliderValue()
		{
            _silderSoundsVolume.value = PlayerPrefs.GetFloat("SoundsVolumeSave", 1);
            _slidersSettings.SliderFillVolume.fillAmount = _silderSoundsVolume.value;

            //  Debug.Log(PlayerPrefs.GetFloat("MusicVolumeSave"));
            _silderMusicVolume.value = PlayerPrefs.GetFloat("MusicVolumeSave", 1);
            _slidersSettings.SliderFillMusic.fillAmount = _silderMusicVolume.value;
        }

      /*  public void ToogleMusic(bool enabled)
        {
            if (enabled)
            {
                _audioMixerGroup.audioMixer.SetFloat("MusicVolume", maxVolumeValue);
            }
            else
            {
                _audioMixerGroup.audioMixer.SetFloat("MusicVolume", minVolumeValue);
            }

            PlayerPrefs.SetInt("MusicEnabled", enabled ? 1 : 0);
        }
      */

        public void ToogleSFX(bool enabled)
        {
            if (enabled)
            {
                _audioMixerGroup.audioMixer.SetFloat("SFXVolume", maxVolumeValue);
            }
            else
            {
                _audioMixerGroup.audioMixer.SetFloat("SFXVolume", minVolumeValue);
            }

            PlayerPrefs.SetInt("SFXEnabled", enabled ? 1 : 0);
        }

        public void SliderMusic(float volume)
        {
            _musicAudioMixer.audioMixer.SetFloat("MusicVolume", Mathf.Lerp(-40, 0, volume));

            PlayerPrefs.SetFloat("MusicVolumeSave", volume);
        }

        public void ChangeVolume(float volume)
        {
            _audioMixerGroup.audioMixer.SetFloat("SoundsVolume", Mathf.Lerp(-40, 0, volume));

            PlayerPrefs.SetFloat("SoundsVolumeSave", volume);
        }

    }
}