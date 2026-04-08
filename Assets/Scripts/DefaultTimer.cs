using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QuizCinema
{
    public class DefaultTimer : MonoBehaviour
    {
        private float _timeTimer = 4f;

        public Slider slider;

        private void OnEnable()
        {
            StartCoroutine(StartTimer());
        }

        IEnumerator StartTimer()
        {
            Debug.Log("Start Timer - «¿œ”—  “¿…Ã≈–¿!");

            float timeLeft = 0;
           // _timerText.color = _timerDefaultColor;

            while (timeLeft <= 1)
            {
                timeLeft += 0.01f;
                slider.value = timeLeft;
                // slider.value = 1
              //  AudioManager.Instance.PlaySound(_countdownSFX);

               // _timerText.text = timeLeft.ToString();
                yield return new WaitForSeconds(0.05f);
            }

            Debug.Log("Start Timer -  ÓÌÂˆ “¿…Ã≈–¿!");
        }
    }
}