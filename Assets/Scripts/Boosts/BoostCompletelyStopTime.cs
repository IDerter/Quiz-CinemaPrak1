using UnityEngine;

namespace QuizCinema
{
	public class BoostCompletelyStopTime : BoostParent, IDependency<Timer>
    {
        [SerializeField] private GameObject _buttonFreeze;
        [SerializeField] private BoostFreezeTime _boostFreezeTime;
        [SerializeField] private Timer _timer;
        [SerializeField] private TimerInLvl _timerInLvl;

        protected override void OnCreateAnswers(Question question)
        {
            if (_buttonBoost.TryGetComponent<BoostUICount>(out var boost))
            {
                _boostSO = boost.GetSetBoostSO;
            }

          //  _buttonBoost.SetActive(true);
           // SwitchInteractable(true, _buttonBoost);
        }

        public override void ActivateBoost(bool everyQuestionActivate)
        {
           // base.ActivateBoost();

            if (!_buttonPress)
            {
                _buttonPress = true;
                // _timer.UpdateTimer(false);
                _timerInLvl.StopSlider();
                _timerInLvl.IsStopTime = true;

                if (!everyQuestionActivate)
                    BoostsManager.UseBoost(_boostSO);
            }
        }

        public void Construct(Timer obj)
        {
            _timer = obj;
        }
    }
}