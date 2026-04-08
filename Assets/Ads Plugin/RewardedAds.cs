using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;
using YG;

public class RewardedAds : SingletonBase<RewardedAds>, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public static event Action<string> RewardOn;

    [SerializeField] private string _androidAdUnityId = "Rewarded_Android";
    [SerializeField] private string _iosAdUnityId = "Rewarded_iOS";

    private string _adUnitId = null;

    [SerializeField] private GameObject _toDelete;
    [SerializeField] private QuizCinema.TypeReward _type;

    private bool _isYandexReady = false;
    private bool _isRewardPending = false;

    protected override void Awake()
    {
        base.Awake();

#if UNITY_IOS
        _adUnitId = _iosAdUnityId;
#elif UNITY_ANDROID
        _adUnitId = _androidAdUnityId;
#endif
    }

    private void Start()
    {
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
        Debug.Log($"[RewardedAds] Yandex SDK ready: {_isYandexReady}");
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

    public void LoadRewardedAd(GameObject toDelete = null)
    {
        _toDelete = toDelete;

        if (YG2.platform == "YandexGames")
        {
            if (!_isYandexReady)
            {
                Debug.Log("[RewardedAds] Yandex SDK not ready yet, waiting...");
                StartCoroutine(LoadWhenYandexReady(toDelete));
                return;
            }

            Debug.Log("[RewardedAds] YandexGames - ready");
            return;
        }

        if (!string.IsNullOrWhiteSpace(_adUnitId))
        {
            Debug.Log("[RewardedAds] Loading Unity Ad");
            Advertisement.Load(_adUnitId, this);
        }
    }

    private IEnumerator LoadWhenYandexReady(GameObject toDelete = null)
    {
        float timeout = 10f;
        float elapsed = 0f;

        while (!IsYandexReady() && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        _isYandexReady = IsYandexReady();
        _toDelete = toDelete;

        if (_isYandexReady)
        {
            Debug.Log("[RewardedAds] Yandex SDK now ready");
        }
    }

    public void ShowRewardedAd(QuizCinema.TypeReward type, GameObject toDelete = null)
    {
        if (_isRewardPending)
        {
            Debug.LogWarning("[RewardedAds] Already showing an ad, cannot show another one");
            return;
        }

        _type = type;
        _toDelete = toDelete;

        if (YG2.platform == "YandexGames")
        {
            if (!_isYandexReady)
            {
                Debug.LogWarning("[RewardedAds] Yandex SDK not ready, cannot show ad");
                return;
            }

            StartCoroutine(ShowYandexRewardedAd());
            return;
        }

        LoadRewardedAd(toDelete);
        StartCoroutine(RewardedDelayShow());
    }

    private IEnumerator ShowYandexRewardedAd()
    {
        _isRewardPending = true;

        Debug.Log($"[RewardedAds] Showing Yandex rewarded ad: {_type}");
        YG2.RewardedAdvShow(_type.ToString());

        // Ждем callback от Яндекс SDK
        yield return new WaitForSeconds(0.5f);

        // Таймаут на случай, если callback не пришел
        float timeout = 10f;
        float elapsed = 0f;

        while (_isRewardPending && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (_isRewardPending)
        {
            Debug.LogWarning("[RewardedAds] Yandex rewarded ad timeout");
            _isRewardPending = false;
        }
    }

    // Этот метод вызывается из Yandex SDK при успешном просмотре
    public void OnRewardedAdReward()
    {
        Debug.Log($"[RewardedAds] Yandex reward received for: {_type}");

        if (_isRewardPending)
        {
            GiveReward();
        }
        else
        {
            Debug.LogWarning("[RewardedAds] Received reward but no pending ad");
        }
    }

    // Этот метод вызывается из Yandex SDK при закрытии рекламы (без награды)
    public void OnRewardedAdClosed()
    {
        Debug.Log("[RewardedAds] Yandex rewarded ad closed without reward");
        _isRewardPending = false;
    }

    private void GiveReward()
    {
        Debug.Log($"[RewardedAds] Giving reward: {_type}");

        if (_toDelete != null)
            Destroy(_toDelete.gameObject, 1f);

        RewardOn?.Invoke(_type.ToString());
        LoadRewardedAd();

        _isRewardPending = false;
    }

    #region IUnityAdsLoadListener implementation
    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log($"[RewardedAds] Unity Ad loaded: {placementId}");
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogWarning($"[RewardedAds] Unity Ad failed to load: {placementId} - {error} - {message}");
        StartCoroutine(RetryLoadAd(5f));
    }
    #endregion

    #region IUnityAdsShowListener implementation
    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.LogError($"[RewardedAds] Unity Ad failed to show: {placementId} - {error} - {message}");
        _isRewardPending = false;
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log($"[RewardedAds] Unity Ad started: {placementId}");
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log($"[RewardedAds] Unity Ad clicked: {placementId}");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log($"[RewardedAds] Unity Ad completed: {placementId} - {showCompletionState}");

        if (placementId == _adUnitId && showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            GiveReward();
        }
        else
        {
            _isRewardPending = false;
        }
    }
    #endregion

    public IEnumerator RewardedDelayShow()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("[RewardedAds] Showing ad after delay");
        ShowAd();
    }

    private IEnumerator RetryLoadAd(float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadRewardedAd();
        Debug.Log($"[RewardedAds] Retrying to load ad in {delay} seconds.");
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
    }
}