using SpaceShooter;
using System;
using TowerDefense;
using UnityEngine;
using UnityEngine.UI;

namespace QuizCinema
{
    public class BoostInGame : BoostUICount
    {
        public event Action OnLearningBooster;

        [SerializeField] protected BoostParent _boostScript;
        [SerializeField] protected Button _buttonBoost;
        [SerializeField] private bool _everyQuestionActivate = false;
        public bool GetEveryQuestionActivate => _everyQuestionActivate;
        [SerializeField] private bool _isSkinBooster = false;
        public bool IsSkinBooster => _isSkinBooster;
        [SerializeField] private GameObject _overlayBooster;
        [SerializeField] private bool _isAllLvlActive = false;
        public bool IsAllLvlActive => _isAllLvlActive;
        [SerializeField] private FadeImage _fadeImage;
        [SerializeField] private bool _isLearningBooster;
        [SerializeField] private LearningLvlManager _learningLvlManager;

        private const string _clickSFX = "ClickSFX";

        private void Start()
        {
            if (GameManager.Instance != null)
			    GameManager.Instance.OnNextQuestion += OnNextQuestion;

            _fadeImage = GetComponent<FadeImage>();
           // if (_boostScript.IsStartActiveBoost)
           //     _boostScript.ActivateBoost();
        }
		private void OnDestroy()
		{
            if (GameManager.Instance != null)
                GameManager.Instance.OnNextQuestion -= OnNextQuestion;
        }

		private void OnNextQuestion()
		{
            if (_everyQuestionActivate)
                _overlayBooster.SetActive(false);
        }

        public void BoostActivate()
        {
            if (!_isSkinBooster && !_isAllLvlActive)
            {
                Transform firstParent = transform.parent;
                if (firstParent != null)
                {
                    // Получаем второго родителя
                    Debug.Log("Компонент найден 1: " + firstParent.name);
                    var myComponent = firstParent.GetComponent<RectTransform>();

                    if (myComponent != null)
                    {
                        //Debug.Log()
                        if (_isLearningBooster && MapCompletion.Instance.LearnSteps[3] && !MapCompletion.Instance.LearnSteps[4])
                            _learningLvlManager.ShowGoodJobAfterClickBooster();

                        AudioManager.Instance.PlaySound(_clickSFX);

                        _fadeImage.FadeOutStartAnim();
                        Destroy(myComponent.gameObject, LevelSequenceController.Instance.TimeAnimClick * 2);
                        Debug.Log("Компонент найден: " + myComponent.ToString());
                    }
                    else
                    {
                        Debug.Log("Компонент не найден на втором родителе.");
                    }
                }

                else
                {
                    Debug.Log("_overlayBooster is true");
                    _overlayBooster.SetActive(true);
                }
            }

            else if (_isSkinBooster)
            {
                Debug.Log("_overlayBooster is true Skin");
                _overlayBooster.SetActive(true);
            }
        }

        /*
		private void OnEnable()
        {
            if (_boostScript.IsStartActiveBoost)
            {
                Debug.Log("Activate Boost!");
                _buttonBoost.interactable = false;

                _boostScript.ActivateBoost(_everyQuestionActivate);
                if (!_isSkinBooster)
                    Destroy(gameObject);
				else
				{
                    _overlayBooster.SetActive(true);
                }
            }
        }
        */
    }
}
