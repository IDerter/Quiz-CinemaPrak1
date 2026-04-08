using System;
using UnityEngine;
using YG;

namespace QuizCinema
{
	public class PurchaseManager : SingletonBase<PurchaseManager>
    {
		public static event Action<string> PurchaseOn;

		private void Start()
		{
			Debug.Log(YG2.purchases[0].priceCurrencyCode.ToString());
		}

		private void OnEnable()
		{
			YG2.onPurchaseSuccess += SuccessPurchased;
			YG2.onPurchaseFailed += FailedPurchased;
		}

		private void OnDisable()
		{
			YG2.onPurchaseSuccess -= SuccessPurchased;
			YG2.onPurchaseFailed -= FailedPurchased;
		}

		private void SuccessPurchased(string id)
		{
			if (id == TypeReward.BuyCoins1.ToString())
			{
				PurchaseOn?.Invoke(TypeReward.BuyCoins1.ToString());

				//YG2.ConsumePurchase(id);
			}

			if (id == TypeReward.Thief.ToString())
			{
				PurchaseOn?.Invoke(TypeReward.Thief.ToString());
			}
		}

		private void FailedPurchased(string id)
		{
			// Покупка не была совершена
		}
	}
}