using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace QuizCinema
{
	public class ClassicRegime : MonoBehaviour
    {
        [Header("Bonus Settings")]
        [SerializeField] private int _costAmount = 100;
        public int CostAmount => _costAmount;

        [Header("UI Elements")]
        public GameObject _classicPanel;
        [SerializeField] private Button _okButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Image _backgroundOverlay;

        [Header("Animation Settings")]
        [SerializeField] private float _animationDuration = 1.5f;
        [SerializeField] private Ease _panelEase = Ease.OutBack;

        [SerializeField] private UICoins _score;
        [SerializeField] private TextMeshProUGUI _textNeedMoney;

        private Vector3 _panelOriginalScale;

        private void Start()
        {
            if (_classicPanel != null)
            {
                _panelOriginalScale = _classicPanel.transform.localScale;
                _classicPanel.SetActive(false);
            }

            if (_backgroundOverlay != null)
            {
                _backgroundOverlay.gameObject.SetActive(false);
                _backgroundOverlay.color = new Color(0, 0, 0, 0);
            }

            // Подписываемся на кнопки
            if (_okButton != null)
                _okButton.onClick.AddListener(ClosePanel);

            if (_closeButton != null)
                _closeButton.onClick.AddListener(ClosePanel);

            // Проверяем бонус при старте
            //CheckDailyBonus();
        }

        private void OnDestroy()
        {
            if (_okButton != null)
                _okButton.onClick.RemoveListener(ClosePanel);

            if (_closeButton != null)
                _closeButton.onClick.RemoveListener(ClosePanel);

        }

        /// <summary>
        /// Показывает панель с не возможностью покупки
        /// </summary>
        public void ShowClassicPanel(int needMoney)
        {
            if (_classicPanel == null) return;

           // StartCoroutine(DelayClassicRegime());

            // Активируем панель
            _classicPanel.SetActive(true);
            _classicPanel.transform.localScale = Vector3.zero;

            // Активируем затемнение
            if (_backgroundOverlay != null)
            {
                _backgroundOverlay.gameObject.SetActive(true);
                _backgroundOverlay.DOFade(0.7f, _animationDuration);
            }

            _textNeedMoney.text = _textNeedMoney.text.Replace("100", $"{needMoney}");

            // Анимация появления панели
            _classicPanel.transform.DOScale(_panelOriginalScale, _animationDuration)
                .SetEase(_panelEase);
        }


        /// <summary>
        /// Закрывает панель и выдает бонус
        /// </summary>
        private void ClosePanel()
        {
            // Анимация закрытия
            Sequence closeSequence = DOTween.Sequence();

            if (_backgroundOverlay != null)
            {
                closeSequence.Join(_backgroundOverlay.DOFade(0f, _animationDuration / 2));
            }

            closeSequence.Join(_classicPanel.transform.DOScale(0f, _animationDuration / 2)
                .SetEase(Ease.InBack));

            closeSequence.OnComplete(() => {
                _classicPanel.SetActive(false);
                if (_backgroundOverlay != null)
                    _backgroundOverlay.gameObject.SetActive(false);
            });
        }

        private IEnumerator DelayClassicRegime()
        {
            yield return new WaitForSeconds(2f);
            BuyClassicRegime();

        }

        /// <summary>
        /// Выдает бонус и сохраняет дату
        /// </summary>
        public void BuyClassicRegime()
        {
            Debug.Log("BuyClassicRegime");
            if (_score != null)
            {
                // Передаем положительное число (100), так как метод сам добавит его к тратам
                _score.SpendCoinsOnClassic(_costAmount);
                Debug.Log($"Вычитаем {_costAmount} монет за участие в классическом режиме!");
            }

        }

    }
}