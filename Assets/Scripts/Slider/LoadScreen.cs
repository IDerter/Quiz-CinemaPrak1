using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace QuizCinema
{
	public class LoadScreen : MonoBehaviour
    {
        [SerializeField] private Image _sliderFill;
        [SerializeField] private Slider _slider;
        [SerializeField] private GameObject _bgLoadScreen;
        [SerializeField] private GameObject _bgBlack;
        [SerializeField] private GameObject _kaeruLogo;
        [SerializeField] private GameObject _particleSystem;
        [SerializeField] private SpawnParticles _spawn;
        private float _timeLeft = 0;

        private const string _sceneMap = "LevelMap";

        private void Start()
        {
            _timeLeft = 0;
            StartCoroutine(BgBlack());
        }

        private IEnumerator BgBlack()
        {
            yield return new WaitForSeconds(1f);

          //  _kaeruLogo.SetActive(true);

        }

        public void DisableBgBlack()
        {
            _bgBlack.GetComponent<Animator>().enabled = true;
        }

        public void ShowBackgroundLoadScreen()
        {
            _bgLoadScreen.SetActive(true);
        }

        public void StartSlider()
        {
            _slider.gameObject.SetActive(true);

            StartCoroutine(StartTimer());
            StartCoroutine(StartParticles());
        }

        IEnumerator StartTimer()
        {
            while (_timeLeft <= 1)
            {
                _timeLeft += 0.01f;
                _sliderFill.fillAmount = _timeLeft;

                if (_slider.value <= 0.72f)
                    _slider.value += 0.008f;

                yield return new WaitForSeconds(0.05f);
            }

            SceneManager.LoadScene(_sceneMap);
        }

        IEnumerator StartParticles()
        {
            while (_timeLeft <= 1)
            {
                _spawn.Spawn();
                yield return new WaitForSeconds(0.8f);
            }
        }
        
    }
}