using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;

public class AdsManager : SingletonBase<AdsManager>
{
	public InitializeAds _initializeAds;
	public BannerAds _bannerAds;
	public InterstitialAds _interstitialAds;
	public RewardedAds _rewardedAds;

	protected override void Awake()
	{
		base.Awake();

		// Начинаем процесс инициализации рекламы
		StartCoroutine(InitializeAdsWhenReady());
	}

	private IEnumerator InitializeAdsWhenReady()
	{
		// Ждем инициализацию Яндекс SDK
		Debug.Log("[AdsManager] Waiting for Yandex SDK initialization...");

		float timeout = 10f;
		float elapsed = 0f;

		// Проверяем инициализацию YG2
		while (!IsYandexReady() && elapsed < timeout)
		{
			elapsed += Time.deltaTime;
			yield return null;
		}

		if (IsYandexReady())
		{
			Debug.Log($"[AdsManager] Yandex SDK initialized. Platform: {YG2.platform}, Lang: {YG2.lang}");
		}
		else
		{
			Debug.LogWarning("[AdsManager] Yandex SDK initialization timeout");
		}

		// Загружаем рекламу
		LoadAllAds();
	}

	private bool IsYandexReady()
	{
		// Проверяем через доступные методы YG2
		try
		{
			// Проверяем, что платформа определена
			if (!string.IsNullOrEmpty(YG2.platform))
			{
				// Проверяем, что язык загружен
				if (!string.IsNullOrEmpty(YG2.lang))
				{
					return true;
				}
			}
		}
		catch
		{
			return false;
		}

		return false;
	}

	private void LoadAllAds()
	{
		Debug.Log("[AdsManager] Loading all ads...");

		if (_bannerAds != null)
			_bannerAds.LoadBannerAd();

		if (_interstitialAds != null)
			_interstitialAds.LoadInterstitialAd();

		if (_rewardedAds != null)
			_rewardedAds.LoadRewardedAd();
	}
}