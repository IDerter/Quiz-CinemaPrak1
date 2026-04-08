using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading.Tasks;
using TMPro;
using TowerDefense;
using UnityEngine;

namespace QuizCinema
{
    public class UICoins : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textCoins;
        [SerializeField] private GameObject _coinImage;

        [Header("Animation Settings")]
        [SerializeField] private float coinsPerSecond = 100f;
        [SerializeField] private float minDuration = 1.5f;
        [SerializeField] private float maxDuration = 3f;

        private int _coins;
        private int _coinsEnd;

        private Tween _coinTween; // Ссылка на твин текста
        private Tween _coinReactionTween; // Ссылка на твин картинки

        private const string _coinSFX = "Coins";
        private bool _isFirstLoad = true;

        private void Start()
        {
            // Устанавливаем моментально без анимации при старте сцены
            SyncCoinsInstant();
        }

        private void OnEnable()
        {
            MapCompletion.OnScoreUpdate += OnScoreUpdate;
            if (UIManager.Instance != null)
                UIManager.Instance.OnFinishScoreCalculating += OnFinishScoreCalculating;
        }

        private void OnDestroy()
        {
            MapCompletion.OnScoreUpdate -= OnScoreUpdate;
            if (UIManager.Instance != null)
                UIManager.Instance.OnFinishScoreCalculating -= OnFinishScoreCalculating;

            // Обязательно убиваем твины при уничтожении объекта, чтобы избежать ошибок null reference
            _coinTween?.Kill();
            _coinReactionTween?.Kill();
        }

        private void SyncCoinsInstant()
        {
            _coinsEnd = (MapCompletion.Instance.TotalScoreLvls + MapCompletion.Instance.TotalAdsMoney) - MapCompletion.Instance.MoneyShop - MapCompletion.Instance.SkinShop - MapCompletion.Instance.MoneySpentClassic;
            _coins = _coinsEnd;
            _textCoins.text = _coins.ToString();
        }

        private async void OnFinishScoreCalculating()
        {
            await CalculateCoins();
        }

        private async void OnScoreUpdate()
        {
            _coinsEnd = (MapCompletion.Instance.TotalScoreLvls + MapCompletion.Instance.TotalAdsMoney) - MapCompletion.Instance.MoneyShop - MapCompletion.Instance.SkinShop - MapCompletion.Instance.MoneySpentClassic;

            // Если это первый апдейт от облака (LoadData), просто моментально ставим значение без долгой анимации с нуля
            if (_isFirstLoad)
            {
                SyncCoinsInstant();
                _isFirstLoad = false;
                return;
            }

            await CalculateScore();
        }

        private async UniTask CalculateScore()
        {
            await CalculateCoins();
        }

        private async Task CalculateCoins()
        {
            float needToSum = _coinsEnd - _coins;
            if (needToSum == 0) return;

            // Если звук проигрывается слишком часто, возможно стоит добавить кулдаун
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySound(_coinSFX);
            }

            var taskReact = ReactCoins();
            var taskAnimateText = AnimateCoinText(needToSum);

            await UniTask.WhenAll(taskReact, taskAnimateText);
        }

        private async UniTask AnimateCoinText(float needToSum)
        {
            float dynamicDuration = Mathf.Clamp(Mathf.Abs(needToSum) / coinsPerSecond, minDuration, maxDuration);

            // ВАЖНО: Убиваем предыдущую анимацию текста, если она еще работает!
            _coinTween?.Kill();

            // Используем целочисленный вариант DOTween, чтобы избежать проблем с плавающей запятой
            _coinTween = DOTween.To(() => _coins, x =>
            {
                _coins = x;
                _textCoins.text = _coins.ToString();
            }, _coinsEnd, dynamicDuration).SetEase(Ease.Linear);

            await _coinTween.ToUniTask();
        }

        private async UniTask ReactCoins()
        {
            if (_coinReactionTween == null || !_coinReactionTween.IsActive() || _coinReactionTween.IsComplete())
            {
                // Убиваем старый твин на всякий случай
                _coinReactionTween?.Kill();

                // Возвращаем скейл в норму перед новой анимацией
                _coinImage.transform.localScale = Vector3.one;

                _coinReactionTween = _coinImage.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 1).SetEase(Ease.InOutElastic);
                await _coinReactionTween.ToUniTask();
            }
        }

        public void AddCoins(int coins)
        {
            MapCompletion.Instance.TotalAdsMoney += coins;
            // Сначала вызываем метод сохранения в облако...
            MapCompletion.SaveAds();
            // ...а OnScoreUpdate вызовется внутри MapCompletion автоматически (у вас так настроено) 
            // или вызываем вручную, если в MapCompletion.SaveAds() нет вызова OnScoreUpdate
            OnScoreUpdate();
        }

        public void SpendCoinsOnClassic(int amount)
        {
            // Прибавляем сумму к общим тратам на классику
            MapCompletion.Instance.MoneySpentClassic += amount;

            // Сохраняем прогресс (YG2.SaveProgress вызовется внутри)
            MapCompletion.SaveAds();

            // Запускаем перерасчет UI
            OnScoreUpdate();
        }
    }
}