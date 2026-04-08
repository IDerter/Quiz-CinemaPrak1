using UnityEngine;
using UnityEngine.Advertisements;

public class BannerAds : MonoBehaviour
{
    [SerializeField] private string _androidAdUnityId;
    [SerializeField] private string _iosAdUnityId;

    private string _adUnitId;

    private void Awake()
    {
        #if UNITY_IOS
            _adUnitId = _iosAdUnityId;
        #elif UNITY_ANDROID
            _adUnitId = _androidAdUnityId;
#endif

        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
    }

    public void LoadBannerAd()
	{
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = BannerLoaded,
            errorCallback = BannerLoadedError
        };

        Advertisement.Banner.Load(_adUnitId, options);
	}

    public void ShowBannerAd()
	{
        BannerOptions options = new BannerOptions
        {
            showCallback = BannerShown,
            clickCallback = BannerClicked,
            hideCallback = BannerHidden,
        };
        Advertisement.Banner.Show(_adUnitId, options);
	}

    public void HideBannerAd()
	{
        Advertisement.Banner.Hide();
	}

	#region ShowCallbacks
	private void BannerHidden()	{	}

	private void BannerClicked() {	}

	private void BannerShown() {	}
	#endregion

	#region LoadCallbacks
	private void BannerLoadedError(string message) {}

	private void BannerLoaded()
	{
        Debug.Log("Banner Ad Loaded");
	}
	#endregion
}
