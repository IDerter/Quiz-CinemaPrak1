using UnityEngine;
using YG;

namespace QuizCinema
{
    public class RewardedManager : MonoBehaviour
    {
        [SerializeField] private UICoins _score;

        private void OnEnable()
        {
            // Подписываемся в зависимости от того, прогрузился ли Яндекс
            if (YG2.platform == "YandexGames" || YG2.isSDKEnabled)
            {
                YG2.onRewardAdv += RewardOn;
                PurchaseManager.PurchaseOn += PurchaseOn;
            }
            else
            {
                RewardedAds.RewardOn += RewardOn;
            }
        }

        private void OnDisable()
        {
            // ГЕНИАЛЬНО ПРОСТО: Отписываемся от ВСЕГО без условий.
            // Если мы не были подписаны на что-то из этого, C# просто проигнорирует строку.
            // Это гарантирует, что никаких скрытых подписок не останется при перезагрузках!
            YG2.onRewardAdv -= RewardOn;
            PurchaseManager.PurchaseOn -= PurchaseOn;
            RewardedAds.RewardOn -= RewardOn;
        }

        private void PurchaseOn(string type)
        {
            Debug.Log($"[SHOP] Покупка успешна: {type}");

            if (type == TypeReward.BuyCoins1.ToString())
            {
                _score.AddCoins(2000);
                Debug.Log($"PURCHASE BUYCOINS! UPDATE SCORE YANDEX GAME : 2000");
            }
        }

        // Этот метод можно удалить, если он нигде не используется, 
        // так как события передают строку (type).
        /*
        private void RewardOn()
        {
            _score.AddCoins(50);
            Debug.Log($"UPDATE SCORE : 50");
        }
        */

        private void RewardOn(string type)
        {
            Debug.Log($"[REWARD] Получена награда за рекламу: {type}");

            if (type == TypeReward.FiftyCoins.ToString())
            {
                _score.AddCoins(50);
                Debug.Log($"UPDATE SCORE YANDEX GAME : 50");
            }
        }
    }
}