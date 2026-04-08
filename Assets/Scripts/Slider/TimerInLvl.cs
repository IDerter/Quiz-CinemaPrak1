using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuizCinema
{
    public class TimerInLvl : FillSlider
    {
        [SerializeField] private QuestionMethods _questionMethods;
        [SerializeField] private bool _isStopTime = false;
        public bool IsStopTime { get { return _isStopTime; } set { _isStopTime = value; } }

        private void Start()
        {
            if (!_isStopTime)
                StartTimer();
        }

        public void StartTimer()
        {
            Debug.Log($"[TimerInLvl] StartTimer called. IsStopTime: {IsStopTime}");
            // Если таймер уже запущен, не делаем ничего
            if (IsSliderRunning)
            {
                Debug.Log($"[{gameObject.name}] Timer already running, ignoring StartTimer()");
                return;
            }

            // Если время остановлено, не запускаем таймер
            if (_isStopTime)
            {
                Debug.Log($"[{gameObject.name}] Timer is stopped, not starting");
                return;
            }

            InitiliazeSlider();
            StartSlider();
        }

        public void ResetAndStartTimer()
        {
            // Полный сброс таймера
            StopSlider();
            _isStopTime = false;
            InitiliazeSlider();
            StartSlider();
        }

        // Метод для принудительной остановки с сохранением состояния
        public void PauseTimer()
        {
            if (IsSliderRunning)
            {
                StopSlider();
            }
        }

        // Метод для возобновления таймера
        public void ResumeTimer()
        {
            if (!IsSliderRunning && !_isStopTime)
            {
                StartTimer();
            }
        }
    }
}