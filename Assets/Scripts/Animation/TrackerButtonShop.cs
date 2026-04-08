using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;

using TMPro;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

namespace QuizCinema
{
    public class TrackerButtonShop : MonoBehaviour
    {
        [SerializeField] private GameObject _panelCoins;

        [SerializeField] private GameObject _panelSkins;

        [SerializeField] private GameObject _panelBoosters;

        [SerializeField] private Transform _chooseImage;

        [SerializeField] private float duration = 2f;
        [SerializeField] private Button _buttonOffers;
        [SerializeField] private Button _buttonSkins;
        [SerializeField] private Button _buttonCoins;
        [SerializeField] private Button _buttonBoosters;

        [SerializeField] private ScrollBarOverlay _scrollSkinsPanel;
        [SerializeField] private ScrollBarOverlay _scrollBoostersPanel;

        private const string _clickSwipeAudio = "ClickSwipe";

        private float _sizeScaleButton = 1.2f;

		private void OnEnable()
		{
            ResetScrollPanels();
        }

		private void Start()
        {
            _buttonSkins.onClick.AddListener(() => MoveToButton(_buttonSkins));
            _buttonOffers.onClick.AddListener(() => MoveToButton(_buttonOffers));
            _buttonCoins.onClick.AddListener(() => MoveToButton(_buttonCoins));
            _buttonBoosters.onClick.AddListener(() => MoveAndScaleButton(_buttonBoosters));
        }

        public void InitState()
		{
            _buttonSkins.GetComponent<UpTextShop>().DisableText();
            _buttonOffers.GetComponent<UpTextShop>().DisableText();
            _buttonCoins.GetComponent<UpTextShop>().DisableText();
            _buttonBoosters.GetComponent<UpTextShop>().DisableText();
        }

        public void MoveToButton(Button button)
		{
            ResetScrollPanels();
            InitState();

            AudioManager.Instance.PlaySound(_clickSwipeAudio);

            _chooseImage.transform.DOMove(new Vector3(button.GetComponent<RectTransform>().position.x, _chooseImage.transform.position.y), duration)
				.SetEase(Ease.Linear).ToUniTask();

			_chooseImage.transform.DOScale(new Vector3(button.GetComponent<RectTransform>().localScale.x, button.GetComponent<RectTransform>().localScale.y), duration)
				.SetEase(Ease.Linear).ToUniTask();

            button.GetComponent<UpTextShop>().ActivateText();
        }

		private void ResetScrollPanels()
		{
		}

		public void MoveAndScaleButton(Button button)
        {
            InitState();

            AudioManager.Instance.PlaySound(_clickSwipeAudio);

            _chooseImage.transform.DOMove(new Vector3(button.GetComponent<RectTransform>().position.x, _chooseImage.transform.position.y), duration)
                .SetEase(Ease.Linear).ToUniTask();

            _chooseImage.transform.DOScale(new Vector3(_sizeScaleButton, button.GetComponent<RectTransform>().localScale.y), duration)
                .SetEase(Ease.Linear).ToUniTask();

            button.GetComponent<UpTextShop>().ActivateText();
        }
    }
}