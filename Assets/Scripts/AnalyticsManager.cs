using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

public class AnalyticsManager : SingletonBase<AnalyticsManager>
{
    private bool _isInitialized = false;

    private async void Start()
    {
        #if !UNITY_EDITOR
        await UnityServices.InitializeAsync();
        AnalyticsService.Instance.StartDataCollection();
        _isInitialized = true;
        #endif
        Debug.Log(_isInitialized);
    }
    
    public void NextLevelStats(int currentLevel)
	{
        #if !UNITY_EDITOR
        Debug.Log(_isInitialized + " nextLevel");
        if (!_isInitialized)
		{
            return;
		}

        CustomEvent myEvent = new CustomEvent("next_level")
        {
            {"level_index", currentLevel }
        };
        AnalyticsService.Instance.RecordEvent(myEvent);
        AnalyticsService.Instance.Flush();
        #endif
        Debug.Log("next_level");
	}

    public void RestartLeveStats(int scene_index)
	{
#if !UNITY_EDITOR
        CustomEvent myEvent = new CustomEvent("restart_level")
        {
            {"scene_index", scene_index }
        };

        AnalyticsService.Instance.RecordEvent(myEvent);
        AnalyticsService.Instance.Flush();
#endif

        Debug.Log("restart_level");
    }

    public void SaveLevelStarsStats(int scene_index, int stars_count)
    {
#if !UNITY_EDITOR
        CustomEvent myEvent = new CustomEvent("stars_level")
        {
            {"scene_index", scene_index },
            {"stars_count", stars_count }
        };

        AnalyticsService.Instance.RecordEvent(myEvent);
        AnalyticsService.Instance.Flush();
#endif

        Debug.Log($"stars_level уровень = {scene_index} и кол-во звезд = {stars_count}");
    }

    public void SaveClassicStats(int scene_index, int answersCount, int answersMaxRecord)
    {
#if !UNITY_EDITOR
        CustomEvent myEvent = new CustomEvent("classic_stats")
        {
            {"scene_index", scene_index },
            {"answers_count", answersCount },
            {"answers_max_record", answersMaxRecord }
        };

        AnalyticsService.Instance.RecordEvent(myEvent);
        AnalyticsService.Instance.Flush();
#endif

        Debug.Log($"Кол-во ответов за классический режим = {answersCount} и рекорд = {answersMaxRecord}");
    }

    public void SaveErrorInLvl(int scene_index, string message)
    {
#if !UNITY_EDITOR
        CustomEvent myEvent = new CustomEvent("error_level")
        {
            {"scene_index", scene_index },
            {"message_error", message }
        };

        AnalyticsService.Instance.RecordEvent(myEvent);
        AnalyticsService.Instance.Flush();
#endif

        Debug.Log($"Произошла ошибка на сцене с индексом = {scene_index} и сообщением = {message}");
    }

    public void SaveRewardedAds(string type)
    {
#if !UNITY_EDITOR
        CustomEvent myEvent = new CustomEvent("rewarded_ads")
        {
            {"rewarded_type", type }
        };

        AnalyticsService.Instance.RecordEvent(myEvent);
        AnalyticsService.Instance.Flush();
#endif

        Debug.Log($"SaveRewardedAds {type}");
    }

    public void SaveLearningStep(string stepName)
    {
#if !UNITY_EDITOR
        CustomEvent myEvent = new CustomEvent("stepLearning")
        {
            {"step_name", stepName }
        };

        AnalyticsService.Instance.RecordEvent(myEvent);
        AnalyticsService.Instance.Flush();
#endif

        Debug.Log($"stepName = {stepName}");
    }

    public void ShowRatePanelAndroid()
    {
#if !UNITY_EDITOR
        CustomEvent myEvent = new CustomEvent("ratePanel");

        AnalyticsService.Instance.RecordEvent(myEvent);
        AnalyticsService.Instance.Flush();
#endif

        Debug.Log($"ShowRatePanelAndroid");
    }

    public void SaveNameSkin(string skinName)
    {
#if !UNITY_EDITOR
        CustomEvent myEvent = new CustomEvent("skins")
        {
            {"skin_name", skinName }
        };

        AnalyticsService.Instance.RecordEvent(myEvent);
        AnalyticsService.Instance.Flush();
#endif

        Debug.Log($"skinName = {skinName}");
    }

    public void SaveLikeMovie(string movieName)
    {
#if !UNITY_EDITOR
        CustomEvent myEvent = new CustomEvent("movieLikes")
        {
            {"movie_name", movieName }
        };

        AnalyticsService.Instance.RecordEvent(myEvent);
        AnalyticsService.Instance.Flush();
#endif

        Debug.Log($"movieName = {movieName}");
    }

    public void SaveDownloadError(string error)
    {
#if !UNITY_EDITOR
        CustomEvent myEvent = new CustomEvent("downloadErrors")
        {
            {"download_error", error }
        };

        AnalyticsService.Instance.RecordEvent(myEvent);
        AnalyticsService.Instance.Flush();
#endif

        Debug.Log($"download error = {error}");
    }

    public void SaveLowDevice(string modelName)
    {
#if !UNITY_EDITOR
        CustomEvent myEvent = new CustomEvent("lowDevice")
        {
            {"device_name", modelName }
        };

        AnalyticsService.Instance.RecordEvent(myEvent);
        AnalyticsService.Instance.Flush();
#endif

        Debug.Log($"device_namer = {modelName}");
    }

    public void SaveMapS3Load()
    {
#if !UNITY_EDITOR
        CustomEvent myEvent = new CustomEvent("loadMapS3")
        {
        };

        AnalyticsService.Instance.RecordEvent(myEvent);
        AnalyticsService.Instance.Flush();
#endif

        Debug.Log($"SaveMapS3Load");
    }

    public void SaveDailyBonus()
    {
#if !UNITY_EDITOR
        CustomEvent myEvent = new CustomEvent("dailyBonus")
        {
        };

        AnalyticsService.Instance.RecordEvent(myEvent);
        AnalyticsService.Instance.Flush();
#endif

        Debug.Log($"SaveDailyBonus");
    }
}
