using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace QuizCinema
{
    public class Timer : MonoBehaviour, IDependency<QuestionMethods>, IDependency<TextMeshProUGUI>, IDependency<GameManager>
    {
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private QuestionMethods _questionMethods;
        [SerializeField] private Animator _timerAnimator;
        public Animator TimerAnimator { get { return _timerAnimator; } set { value = _timerAnimator; } }

        [SerializeField] private TextMeshProUGUI _timerText;

        private Color _timerDefaultColor = Color.white;

        private const string _countdownSFX = "CountdownSFX";

        private int _timerStateParaHash = 0;
        public int GetTimerStateParaHash => _timerStateParaHash;

        private IEnumerator IE_StartTimer = null;
        public IEnumerator GetStartTimer { get { return IE_StartTimer; } set { value = IE_StartTimer; } }

        // Флаг для проверки активного таймера
        private bool _isTimerRunning = false;
        public bool IsTimerRunning => _isTimerRunning;

        private void Start()
        {
            _timerStateParaHash = Animator.StringToHash("Timer State");
        }

        public void UpdateTimer(bool state)
        {
            switch (state)
            {
                case true:
                    // Проверяем, не запущен ли уже таймер
                    if (_isTimerRunning)
                    {
                        Debug.LogWarning("Timer is already running! Ignoring start request.");
                        return;
                    }

                    IE_StartTimer = StartTimer();
                    StartCoroutine(IE_StartTimer);
                    _timerAnimator.SetInteger(_timerStateParaHash, 0);
                    _isTimerRunning = true;
                    break;

                case false:
                    if (IE_StartTimer != null)
                    {
                        StopCoroutine(IE_StartTimer);
                        IE_StartTimer = null;
                    }

                    _timerAnimator.SetInteger(_timerStateParaHash, 2);
                    _isTimerRunning = false;
                    break;
            }
        }

        IEnumerator StartTimer()
        {
            Debug.Log("Start Timer - ЗАПУСК ТАЙМЕРА!");
            var totalTime = _questionMethods.Data.Questions[_questionMethods._currentIndexNotRandom].Timer;
            var timeLeft = totalTime;
            _timerText.color = _timerDefaultColor;

            while (timeLeft > 0)
            {
                timeLeft--;
                _timerText.text = timeLeft.ToString();
                yield return new WaitForSeconds(1f);
            }

            _gameManager.Accept();
            _isTimerRunning = false; // Сбрасываем флаг после завершения
        }

        public void Construct(QuestionMethods obj)
        {
            _questionMethods = obj;
        }

        public void Construct(TextMeshProUGUI obj)
        {
            _timerText = obj;
        }

        public void Construct(GameManager obj)
        {
            _gameManager = obj;
        }
    }
}