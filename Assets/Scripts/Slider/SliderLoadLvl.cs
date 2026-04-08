using System.Collections;
using UnityEngine;
using System;

namespace QuizCinema
{
    public class SliderLoadLvl : FillSlider
    {
        public event Action OnSliderCompleted;

        private IEnumerator IE_SliderLvl = null;
        public IEnumerator GetIESliderLvl => IE_SliderLvl;

        public void StartSliderLoadLvl()
		{
            _time = GameManager.Instance.GetTimeLoadLvl - 0.5f;
            _delay = 0.05f;

            InitiliazeSlider();
            StartSlider();
        }

        public override void StartSlider()
        {
            // ѕровер€ем, инициализирована ли корутина
            if (IE_SliderProgress == null)
            {
                Debug.LogWarning($"[{gameObject.name}] Slider not initialized! Calling InitiliazeSlider()");
                InitiliazeSlider();
            }

            if (IE_SliderProgress != null)
            {
                _soundPlayed = false; // —брасываем флаг звука при новом запуске
                StartCoroutine(IE_SliderProgress);
                _isSliderRunning = true;
                Debug.Log($"[{gameObject.name}] Slider started.");
            }
        }

        protected override IEnumerator FillProgressSlider(float delay, float time)
        {
            Debug.Log($"[FillSlider] Coroutine STARTED. Time: {Time.time}");

            float timeLeft = 0;
            _fiilLoadingBar.fillAmount = 0; // Ёто поле из родительского класса FillSlider
            Debug.Log(_fiilLoadingBar.fillAmount);

            while (timeLeft <= time)
            {
                timeLeft += Time.deltaTime;
                _fiilLoadingBar.fillAmount = timeLeft / time;
                yield return null;
            }

            Debug.Log("OnSliderCompleted");
            OnSliderCompleted?.Invoke();

            StartCoroutine(DelayFillAmountZero());

            Debug.Log($"[FillSlider] Coroutine ENDED at time {Time.time}. Calling OnEndFillSlider");
        }

        private IEnumerator DelayFillAmountZero()
		{
            yield return new WaitForSeconds(1f);

            _fiilLoadingBar.fillAmount = 0;
        }
    }
}