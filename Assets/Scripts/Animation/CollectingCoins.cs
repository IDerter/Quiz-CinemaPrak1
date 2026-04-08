using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using TMPro;
using TowerDefense;
using UnityEngine;
using Random = UnityEngine.Random;

namespace QuizCinema
{
	public class CollectingCoins : SingletonBase<CollectingCoins>
    {
        [SerializeField] private GameObject coinPrefab;

        [SerializeField] private Transform coinParent;

        [SerializeField] private Transform spawnLocation;

        [SerializeField] private Transform endPosition;
        [SerializeField] private Transform endPositionToPunch;
        [SerializeField] private Transform _cointUpper;

        [SerializeField] private TextMeshProUGUI _coinText;
        [Header("Animation Settings")]
        [SerializeField] private float coinsPerSecond = 100f;
        [SerializeField] private float minDuration = 1.5f;
        [SerializeField] private float maxDuration = 3f;
        [SerializeField] private int coinAmount;

        [SerializeField] private float minX;

        [SerializeField] private float maxX;

        [SerializeField] private float minY;

        [SerializeField] private float maxY;

        List<GameObject> coins = new List<GameObject>();

        private Tween coinReactionTween;

        [SerializeField] private float coinStart;
        [SerializeField] private float coinEnd;
        [SerializeField] private float needToSum;

        private const string _coinSFX = "Coins";


        private void Start()
        {
            
            MapCompletion.OnScoreUpdate += OnScoreUpdate;
            coinStart = (MapCompletion.Instance.TotalScoreLvls + MapCompletion.Instance.TotalAdsMoney) - MapCompletion.Instance.MoneyShop - MapCompletion.Instance.SkinShop;

            _coinText.text = coinStart.ToString();
        }

        private void OnDestroy()
        {
            MapCompletion.OnScoreUpdate -= OnScoreUpdate;
        }

        private void OnScoreUpdate()
        {
            coinEnd = (MapCompletion.Instance.TotalScoreLvls + MapCompletion.Instance.TotalAdsMoney) - MapCompletion.Instance.MoneyShop - MapCompletion.Instance.SkinShop;
            needToSum = coinEnd - coinStart;
           // _coinText.text = (coinEnd - coinStart).ToString();
            if (needToSum > 0)
                CollectCoins();

        }

        [Button()]
        public async void CollectCoins()
        {
            // Reset
            
            for (int i = 0; i < coins.Count; i++)
            {
                Destroy(coins[i]);
            }
            coins.Clear();
            // Spawn the coin to a specific location with random value

            List<UniTask> spawnCoinTaskList = new List<UniTask>();
            AudioManager.Instance.PlaySound(_coinSFX);
            for (int i = 0; i < coinAmount; i++)
            {
                GameObject coinInstance = Instantiate(coinPrefab, coinParent);
                float xPosition = spawnLocation.localPosition.x + Random.Range(minX, maxX);
                float yPosition = spawnLocation.localPosition.y + Random.Range(minY, maxY);

                coinInstance.transform.localPosition = new Vector3(xPosition, yPosition);
                
                spawnCoinTaskList.Add(coinInstance.transform.DOLocalMove(new Vector3(0, 30, 0), 0.5f).SetRelative(true).SetEase(Ease.OutQuad).ToUniTask());
                coins.Add(coinInstance);
            }

            await UniTask.WhenAll(spawnCoinTaskList);
            // Move all the coins to the coin label and animate text
            await UniTask.WhenAll(MoveCoinsTask(), AnimateCoinText());
            // Animation the reaction when collecting coin
        }

        private void SetCoin(float value)
        {
            coinStart = value;
            _coinText.text = ((int)coinStart).ToString();
        }

        private async UniTask AnimateCoinText()
        {
            float dynamicDuration = Mathf.Clamp(needToSum / coinsPerSecond, minDuration, maxDuration);
            await DOTween.To(() => coinStart, x => SetCoin(x), coinEnd, dynamicDuration).SetEase(Ease.Linear).ToUniTask();
        }

        private async UniTask MoveCoinsTask()
        {
            List<UniTask> moveCoinTasks = new List<UniTask>();
            for (int i = 0; i < coins.Count; i++)
            {
                // Добавляем небольшую случайную задержку для каждой монеты, чтобы они не летели все вместе
                float delay = Random.Range(0, 0.5f);
                bool isLast = (i == coins.Count - 1);
                moveCoinTasks.Add(MoveCoinTask(coins[i], delay, isLast));
            }
            await UniTask.WhenAll(moveCoinTasks);
            coins.Clear();
        }

        private async UniTask MoveCoinTask(GameObject coinInstance, float delay, bool isLast)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay));
            float dynamicDuration = Mathf.Clamp(needToSum / coinsPerSecond, minDuration, maxDuration);

            if (coinInstance == null) return;

            await coinInstance.transform.DOMove(endPosition.position, dynamicDuration).SetEase(Ease.InBack).ToUniTask();

            // Звук и реакция происходят один раз, когда последняя монета долетела
            if (isLast)
            {
                AudioManager.Instance.PlaySound(_coinSFX);
                await ReactToCollectionCoin();
            }
            
            if (coinInstance != null)
            {
                Destroy(coinInstance);
            }
        }

        private async UniTask ReactToCollectionCoin()
        {
            if (coinReactionTween == null)
            {
                coinReactionTween = endPositionToPunch.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.1f).SetEase(Ease.InOutElastic);
                await coinReactionTween.ToUniTask();
                coinReactionTween = null;
            }
            //coinStart = coinEnd;
        }
    }
}