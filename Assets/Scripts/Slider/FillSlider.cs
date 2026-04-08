using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace QuizCinema
{
    public abstract class FillSlider : MonoBehaviour
    {
        public event Action OnEndFillSlider;

        [SerializeField] protected Image _fiilLoadingBar;
        [SerializeField] private float _step = 0.00001f;
        [SerializeField] protected float _delay = 1f;
        [SerializeField] protected float _time = 10f;
        [SerializeField] private AudioClip _audioTimer;
        [SerializeField] private bool _playSoundOnEnd = true; // Флаг для воспроизведения звука только в конце

        protected IEnumerator IE_SliderProgress = null;
        public IEnumerator GetIESliderProgress => IE_SliderProgress;

        // Флаг для защиты от множественного запуска
        protected bool _isSliderRunning = false;
        public bool IsSliderRunning => _isSliderRunning;

        // Флаг для отслеживания, был ли уже воспроизведен звук окончания
        protected bool _soundPlayed = false;

        public virtual void InitiliazeSlider()
        {
            _fiilLoadingBar.fillAmount = 0;
            // Останавливаем предыдущую корутину, если она была
            if (IE_SliderProgress != null)
            {
                StopCoroutine(IE_SliderProgress);
                IE_SliderProgress = null;
            }

            IE_SliderProgress = FillProgressSlider(_delay, _time);
        }

        public virtual void StartSlider()
        {
            // Проверяем флаг остановки времени
            if (this is TimerInLvl timer && timer.IsStopTime)
            {
                Debug.Log($"[{gameObject.name}] Timer is stopped by IsStopTime flag");
                return;
            }

            // Если слайдер уже запущен, просто выходим
            if (_isSliderRunning)
            {
                Debug.LogWarning($"[{gameObject.name}] Slider is already running! Ignoring start request.");
                return;
            }

            // Проверяем, инициализирована ли корутина
            if (IE_SliderProgress == null)
            {
                Debug.LogWarning($"[{gameObject.name}] Slider not initialized! Calling InitiliazeSlider()");
                InitiliazeSlider();
            }

            if (IE_SliderProgress != null)
            {
                _soundPlayed = false; // Сбрасываем флаг звука при новом запуске
                StartCoroutine(IE_SliderProgress);
                _isSliderRunning = true;
                Debug.Log($"[{gameObject.name}] Slider started.");
            }
        }

        public virtual void StopSlider()
        {
            if (IE_SliderProgress != null)
            {
                StopCoroutine(IE_SliderProgress);
                IE_SliderProgress = null;
            }

            _isSliderRunning = false;
            _soundPlayed = false; // Сбрасываем флаг звука при остановке
        }

        protected virtual IEnumerator FillProgressSlider(float delay, float time)
        {
            float timeLeft = 0;
            _fiilLoadingBar.fillAmount = 0;

            Debug.Log($"[{gameObject.name}] FillProgressSlider started");

            while (timeLeft <= time)
            {
                // Проверяем флаг остановки во время выполнения
                if (this is TimerInLvl timer && timer.IsStopTime)
                {
                    Debug.Log($"[{gameObject.name}] FillProgressSlider stopped by IsStopTime flag");
                    _isSliderRunning = false;
                    yield break;
                }

                timeLeft += Time.deltaTime;
                _fiilLoadingBar.fillAmount = Mathf.Clamp01(timeLeft / time);

                // Звук больше не воспроизводится в каждом кадре

                yield return null;
            }

            _isSliderRunning = false;

            // Воспроизводим звук только один раз при завершении
            if (_playSoundOnEnd && _audioTimer != null && !_soundPlayed)
            {
                var sound = AudioManager.Instance.GetSoundAudioClip(_audioTimer);
                if (sound != null)
                {
                    sound.Play();
                    _soundPlayed = true;
                }
            }

            Debug.Log($"[{gameObject.name}] FillProgressSlider completed");
            OnEndFillSlider?.Invoke();
        }
    }
}