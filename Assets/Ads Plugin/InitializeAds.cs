using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class InitializeAds : MonoBehaviour, IUnityAdsInitializationListener
{
    [SerializeField] private string _androidGameId;
    [SerializeField] private string _iosGameId;
    [SerializeField] private bool _isTesting;

    private string _gameId;

	public void OnInitializationComplete()
	{
        Debug.Log("Ads Initialized ...");
	}

	public void OnInitializationFailed(UnityAdsInitializationError error, string message)
	{
		
	}

	private void Awake()
	{
        #if UNITY_IOS
            _gameId = _iosGameId;
        #elif UNITY_ANDROID
            _gameId = _androidGameId;
        #elif UNITY_EDITOR
            _gameId = _androidGameId;
        #endif

        if(!Advertisement.isInitialized && Advertisement.isSupported)
		{
            Advertisement.Initialize(_gameId, _isTesting, this);
		}
    }
}
