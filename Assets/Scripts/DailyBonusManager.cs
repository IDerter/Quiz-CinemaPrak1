using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using TowerDefense;
using YG; // Обязательно подключаем плагин Яндекса

namespace QuizCinema
{
    public class DailyBonusManager : MonoBehaviour
    {
        [Header("Bonus Settings")]
        [SerializeField] private int _bonusAmount = 300;

        [Header("UI Elements")]
        [SerializeField] private GameObject _bonusPanel;
        [SerializeField] private Button _okButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Image _backgroundOverlay;

        [Header("Animation Settings")]
        [SerializeField] private float _animationDuration = 1.5f;
        [SerializeField] private Ease _panelEase = Ease.OutBack;

        [SerializeField] private UICoins _score;
        private Vector3 _panelOriginalScale = Vector3.one;

        private void Awake()
        {
            if (_bonusPanel != null)
            {
                _panelOriginalScale = _bonusPanel.activeInHierarchy ?
                    _bonusPanel.transform.localScale : Vector3.one;
            }
        }

        private void OnEnable()
        {
            InitializeUI();
            MapCompletion.OnScoreUpdate += CheckDailyBonus;
        }

        private void Start()
        {
            if (_okButton != null)
                _okButton.onClick.AddListener(ClosePanel);

            if (_closeButton != null)
                _closeButton.onClick.AddListener(ClosePanel);
        }

        private void InitializeUI()
        {
            if (_bonusPanel != null)
            {
                _bonusPanel.SetActive(false);
            }

            if (_backgroundOverlay != null)
            {
                _backgroundOverlay.gameObject.SetActive(false);
                _backgroundOverlay.color = new Color(0, 0, 0, 0);
            }
        }

        private void OnDestroy()
        {
            if (_okButton != null)
                _okButton.onClick.RemoveListener(ClosePanel);

            if (_closeButton != null)
                _closeButton.onClick.RemoveListener(ClosePanel);

            MapCompletion.OnScoreUpdate -= CheckDailyBonus;
        }

        private void CheckDailyBonus()
        {
            // Проверяем, загрузились ли уже данные из облака Яндекса
            if (!YG2.isSDKEnabled) return;

            Debug.Log("CheckDailyBonus");
            if (CanClaimBonus())
            {
                Debug.Log("CheckDailyBonus - true");
                ShowBonusPanel();
            }
        }

        private bool CanClaimBonus()
        {
            // Читаем дату из облачного сохранения Яндекса
            string lastDateString = YG2.saves.lastDailyBonusDate;
            Debug.Log($"Last bonus date (Cloud): {lastDateString}");

            if (string.IsNullOrEmpty(lastDateString))
            {
                Debug.Log("First time bonus");
                return true;
            }

            // Пытаемся распарсить дату. Используем try-catch на случай сбоя формата
            try
            {
                DateTime lastDate = DateTime.Parse(lastDateString);

                // Совет: DateTime.UtcNow лучше, чем DateTime.Now, так как не зависит от часового пояса устройства
                DateTime currentDate = DateTime.UtcNow;

                bool canClaim = (currentDate - lastDate).TotalHours >= 24;
                Debug.Log($"Can claim bonus: {canClaim}");

                return canClaim;
            }
            catch (Exception e)
            {
                Debug.LogError($"Ошибка чтения даты бонуса: {e.Message}");
                // Если данные сломались, разрешаем забрать бонус и перезаписать дату
                return true;
            }
        }

        private void ShowBonusPanel()
        {
            if (_bonusPanel == null) return;

            DOTween.Kill(_bonusPanel.transform);

            _bonusPanel.SetActive(true);
            _bonusPanel.transform.localScale = Vector3.zero;

            if (_backgroundOverlay != null)
            {
                _backgroundOverlay.gameObject.SetActive(true);
                _backgroundOverlay.color = new Color(0, 0, 0, 0);
                _backgroundOverlay.DOFade(0.7f, _animationDuration);
            }

            _bonusPanel.transform.DOScale(_panelOriginalScale, _animationDuration)
                .SetEase(_panelEase)
                .OnStart(() => Debug.Log("Panel animation started"))
                .OnComplete(() => Debug.Log("Panel animation completed"));

            StopAllCoroutines();
            StartCoroutine(DelayGiveBonus());
        }

        private void ClosePanel()
        {
            DOTween.Kill(_bonusPanel.transform);
            if (_backgroundOverlay != null)
                DOTween.Kill(_backgroundOverlay);

            Sequence closeSequence = DOTween.Sequence();

            if (_backgroundOverlay != null)
            {
                closeSequence.Join(_backgroundOverlay.DOFade(0f, _animationDuration / 2));
            }

            closeSequence.Join(_bonusPanel.transform.DOScale(0f, _animationDuration / 2)
                .SetEase(Ease.InBack));

            closeSequence.OnComplete(() => {
                _bonusPanel.SetActive(false);
                if (_backgroundOverlay != null)
                    _backgroundOverlay.gameObject.SetActive(false);
            });

            Debug.Log("ClosePanel");
        }

        private IEnumerator DelayGiveBonus()
        {
            yield return new WaitForSeconds(1f);
            GiveBonus();
        }

        private void GiveBonus()
        {
            // Вызываем аналитику, если она у вас есть
            if (AnalyticsManager.Instance != null)
            {
                AnalyticsManager.Instance.SaveDailyBonus();
            }

            Debug.Log("GiveBonus");
            if (_score != null)
            {
                // Начисляем монеты. Убедитесь, что внутри _score.AddCoins вызывается MapCompletion.SaveAds()
                _score.AddCoins(_bonusAmount);
                Debug.Log($"Ежедневный бонус {_bonusAmount} монет выдан!");
            }

            // Записываем текущую UTC-дату в облако
            YG2.saves.lastDailyBonusDate = DateTime.UtcNow.ToString();

            // Сохраняем прогресс на сервера Яндекса
            YG2.SaveProgress();
        }

        public void ResetBonus()
        {
            // Очищаем облачную переменную и сохраняем
            YG2.saves.lastDailyBonusDate = "";
            YG2.SaveProgress();

            Debug.Log("Ежедневный бонус сброшен (Cloud)");
        }
    }
}