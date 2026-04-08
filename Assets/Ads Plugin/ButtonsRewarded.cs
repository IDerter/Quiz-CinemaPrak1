using UnityEngine;
using UnityEngine.UI;

namespace QuizCinema
{
	public enum TypeReward
	{
        FiftyCoins = 0,
        X1_5 = 1,
        RestartClassicGame = 2,
        BuyCoins1 = 3,
        Thief = 4
	}

    public class ButtonsRewarded : MonoBehaviour
    {
        [SerializeField] private Button _buttonClickRewarded;
        [SerializeField] private bool _isDelete = true;
        [SerializeField] private TypeReward _type;


        public void Multiplier()
		{
            AdsManager.Instance._rewardedAds.ShowRewardedAd(_type, null);

            if (AnalyticsManager.Instance != null)
                AnalyticsManager.Instance.SaveRewardedAds(_type.ToString());

            if(_isDelete)
                Destroy(_buttonClickRewarded.gameObject, 1f);
		}
    }
}