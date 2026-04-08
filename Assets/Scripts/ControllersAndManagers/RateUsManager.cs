#if UNITY_ANDROID
using UnityEngine;
using Google.Play.Review;
using System.Collections;

public class RateUsManager : MonoBehaviour
{
    private const string ReviewRequestedKey = "InAppReviewRequested";
    private ReviewManager _reviewManager;
    private PlayReviewInfo _playReviewInfo;

    void Awake()
    {
        _reviewManager = new ReviewManager();
    }

    public void ShowReview()
    {
        if (PlayerPrefs.GetInt(ReviewRequestedKey, 0) == 1)
        {
            Debug.Log("In-App Review already requested. Skipping.");
            return;
        }

        Debug.Log("Attempting to show In-App Review.");
        StartCoroutine(RequestReview());
    }

    private IEnumerator RequestReview()
    {
        Debug.Log("Requesting In-App Review flow.");
        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;

        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            Debug.LogError($"In-App Review error: {requestFlowOperation.Error.ToString()}");
            yield break;
        }

        // Set the flag immediately after a successful request, before showing the dialog.
        // This ensures we don't ask again even if the user doesn't complete the review.
        PlayerPrefs.SetInt(ReviewRequestedKey, 1);
        PlayerPrefs.Save();
        AnalyticsManager.Instance.ShowRatePanelAndroid();

        Debug.Log("In-App Review flow requested successfully. Flag set to not ask again.");

        _playReviewInfo = requestFlowOperation.GetResult();

        Debug.Log("Launching In-App Review flow.");
        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
        yield return launchFlowOperation;

        _playReviewInfo = null; // Reset the object

        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            Debug.LogError($"In-App Review launch error: {launchFlowOperation.Error.ToString()}");
            yield break;
        }
        
        Debug.Log("In-App Review flow finished.");
        // The flow has finished. The API does not indicate whether the user
        // reviewed or not, or even whether the review dialog was shown. Thus, no
        // matter the result, we continue our app flow.
    }
}
#else
using UnityEngine;
public class RateUsManager : MonoBehaviour
{
    public void ShowReview()
    {
        Debug.Log("In-App Review is only available on Android. This is a mock call.");
    }
}
#endif

