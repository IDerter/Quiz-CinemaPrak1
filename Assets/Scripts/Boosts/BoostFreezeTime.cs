using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QuizCinema
{
    public class BoostFreezeTime : BoostParent
    {
        public static event Action OnStopTime;
        public static event Action OnEndStopTime;
        [SerializeField] private TimerInLvl _timerInLvl;

        [SerializeField] private Image _freezeBoosterIce1;
        [SerializeField] private Image _freezeBoosterIce2;

        private const float _timeFreeze = 10f;

        private IEnumerator _coroutineFreezeHalfTime; 
        private IEnumerator _coroutineFreeze;

        private void Start()
		{
			GameManager.Instance.OnNextQuestion += OnNextQuestion;
		}

		private void OnDestroy()
		{
            GameManager.Instance.OnNextQuestion -= OnNextQuestion;
        }

		private void OnNextQuestion()
		{
            _buttonPress = false;
            if (_freezeBoosterIce1.TryGetComponent<FadeImage>(out var fade))
            {
                fade.FadeOutStartAnim();
            }

            if (_freezeBoosterIce2.TryGetComponent<FadeImage>(out var fade2))
            {
                fade2.FadeOutStartAnim();
            }
        }

		protected override void OnCreateAnswers(Question question)
        {
            if (_coroutineFreeze != null)
                StopCoroutine(_coroutineFreeze);

            if (_coroutineFreezeHalfTime != null)
                StopCoroutine(_coroutineFreezeHalfTime);

            if (_buttonBoost.TryGetComponent<BoostUICount>(out var boost))
            {
                _boostSO = boost.GetSetBoostSO;
            }
            //_timerInLvl.IsStopTime = false;
            // _buttonBoost.SetActive(true);
            //SwitchInteractable(true, _buttonBoost);
        }

        public override void ActivateBoost(bool everyQuestionActivate)
        {
            if (everyQuestionActivate)
                _buttonPress = false;

            _timerInLvl.IsStopTime = false;
            //base.ActivateBoost();

            if (!_buttonPress)
            {
                if (_freezeBoosterIce1.TryGetComponent<FadeImage>(out var fade))
                {
                    fade.FadeInStartAnim();
				}

                if (_freezeBoosterIce2.TryGetComponent<FadeImage>(out var fade2))
                {
                    fade2.FadeInStartAnim();
                }


                _timerInLvl.StopCoroutine(_timerInLvl.GetIESliderProgress);
               // _timer.StopCoroutine(_timer.GetStartTimer);
                _buttonPress = true;

                _coroutineFreeze = StartFreezeTime();
                StartCoroutine(_coroutineFreeze);

                _coroutineFreezeHalfTime = StartFreezeTimeHalfTime();
                StartCoroutine(_coroutineFreezeHalfTime);

                if (!everyQuestionActivate)
                    BoostsManager.UseBoost(_boostSO);
            }
        }

        IEnumerator StartFreezeTime()
        {
            yield return new WaitForSeconds(_timeFreeze);

            if (!_timerInLvl.IsStopTime)
            {
                _timerInLvl.StartCoroutine(_timerInLvl.GetIESliderProgress);
            }

            if (_freezeBoosterIce2.TryGetComponent<FadeImage>(out var fade2))
            {
                fade2.FadeOutStartAnim();
            }
            _buttonPress = false;
        }

        IEnumerator StartFreezeTimeHalfTime()
        {
            yield return new WaitForSeconds(_timeFreeze / 2);

            if (_freezeBoosterIce1.TryGetComponent<FadeImage>(out var fade))
            {
                fade.FadeOutStartAnim();
            }
        }
    }
}