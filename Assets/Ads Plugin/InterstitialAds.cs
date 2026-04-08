using System;
using System.Collections;
using TowerDefense;
using UnityEngine;
using UnityEngine.Advertisements;
using YG;

public class InterstitialAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public static event Action OnInterstitialAdClosed;

    [SerializeField] private string _androidAdUnityId;
    [SerializeField] private string _iosAdUnityId;

    private string _adUnitId;
    private bool _isYandexReady = false;

    private void Awake()
    {
#if UNITY_IOS
            _adUnitId = _iosAdUnityId;
#elif UNITY_ANDROID
            _adUnitId = _androidAdUnityId;
#endif
    }

    private void Start()
    {
        // Проверяем готовность Яндекс SDK
        if (YG2.platform == "YandexGames")
        {
            StartCoroutine(WaitForYandexSDK());
        }
        else
        {
            _isYandexReady = true;
        }
    }

    private IEnumerator WaitForYandexSDK()
    {
        float timeout = 10f;
        float elapsed = 0f;

        while (!IsYandexReady() && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        _isYandexReady = IsYandexReady();
        Debug.Log($"[InterstitialAds] Yandex SDK ready: {_isYandexReady}");
    }

    private bool IsYandexReady()
    {
        try
        {
            if (!string.IsNullOrEmpty(YG2.platform) && !string.IsNullOrEmpty(YG2.lang))
            {
                return true;
            }
        }
        catch
        {
            return false;
        }

        return false;
    }

    public void LoadInterstitialAd()
    {
        if (YG2.platform == "YandexGames")
        {
            if (!_isYandexReady)
            {
                Debug.Log("[InterstitialAds] Yandex SDK not ready yet, waiting...");
                StartCoroutine(LoadWhenYandexReady());
                return;
            }

            Debug.Log("[InterstitialAds] YandexGames - using Yandex SDK");
            return;
        }

        if (!string.IsNullOrWhiteSpace(_adUnitId))
        {
            Debug.Log("[InterstitialAds] Loading Unity Ads");
            Advertisement.Load(_adUnitId, this);
        }
    }

    private IEnumerator LoadWhenYandexReady()
    {
        float timeout = 10f;
        float elapsed = 0f;

        while (!IsYandexReady() && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        _isYandexReady = IsYandexReady();

        if (_isYandexReady)
        {
            Debug.Log("[InterstitialAds] Yandex SDK now ready");
        }
    }

    public void ShowInterstitialAd()
    {
        if (YG2.platform == "YandexGames")
        {
            if (!_isYandexReady)
            {
                Debug.Log("[InterstitialAds] Yandex SDK not ready, cannot show ad");
                OnInterstitialAdClosed?.Invoke();
                return;
            }

            StartCoroutine(ShowYandexInterstitial());
            return;
        }

        LoadInterstitialAd();
        StartCoroutine(InterstitialDelayShow());
    }

    private IEnumerator ShowYandexInterstitial()
    {
        Debug.Log("[InterstitialAds] Showing Yandex interstitial");
        YG2.InterstitialAdvShow();

        yield return new WaitForSeconds(0.5f);
        OnInterstitialAdClosed?.Invoke();
    }

    public void ShowAd()
    {
        if (YG2.platform == "YandexGames")
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(_adUnitId))
        {
            Advertisement.Show(_adUnitId, this);
        }
        else
        {
            OnInterstitialAdClosed?.Invoke();
        }
    }

    #region IUnityAdsLoadListener implementation
    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log($"[InterstitialAds] Unity Ads loaded: {placementId}");
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogWarning($"[InterstitialAds] Unity Ads failed to load: {placementId} - {error} - {message}");
        OnInterstitialAdClosed?.Invoke();
        StartCoroutine(RetryLoadAd(5f));
    }
    #endregion

    #region IUnityAdsShowListener implementation
    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.LogError($"[InterstitialAds] Unity Ads failed to show: {placementId} - {error} - {message}");
        OnInterstitialAdClosed?.Invoke();
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log($"[InterstitialAds] Unity Ads started: {placementId}");
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log($"[InterstitialAds] Unity Ads clicked: {placementId}");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log($"[InterstitialAds] Unity Ads completed: {placementId} - {showCompletionState}");

        if (placementId == _adUnitId)
        {
            OnInterstitialAdClosed?.Invoke();
        }
    }
    #endregion

    public IEnumerator InterstitialDelayShow()
    {
        yield return new WaitForSeconds(2f);
        ShowAd();
        MapCompletion.SaveLvlFinished();
    }

    private IEnumerator RetryLoadAd(float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadInterstitialAd();
        Debug.Log($"[InterstitialAds] Retrying to load ad in {delay} seconds.");
    }
}