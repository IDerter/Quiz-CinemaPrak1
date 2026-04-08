using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace QuizCinema
{
	public class BuyBoost : BuyParent
    {
        [SerializeField] private BoostSO _asset;
        public BoostSO BoostSO { get { return _asset; } set { _asset = value ; } }
        [SerializeField] private Image _upgradeIcon;
        [SerializeField] private TextMeshProUGUI _textCount;
        [SerializeField] private TextMeshProUGUI _textCost;
        [SerializeField] private Button _buttonBuyBoost;
        [SerializeField] private InteractableButton _viewButton;
        //public Button GetButton => _buttonBuy;
        [SerializeField] private int _numberOfBoosters = 1;
        [SerializeField] private GameObject _overlayButton;
        [SerializeField] private GameObject _overlayCardBooster;

        private int _costNumber; // цена покупки улучшения

        private const string _coinSFX = "Coins";

        private void Awake()
        {
            Initialize();

            if (ButtonBuy == null)
                ButtonBuy = _buttonBuyBoost;
            if (_buttonBuyBoost == null)
                _buttonBuyBoost = ButtonBuy;
        }

        public override void Initialize()
        {
            _upgradeIcon.sprite = _asset.sprite;
            // var savedLevel = BoostsManager.GetUpgradeLevel(_asset);
            if (_textCount != null)
                _textCount.text = BoostsManager.GetCountBoost(_asset).ToString();

            _costNumber = _asset.DictionaryNumberOfBoosts[_numberOfBoosters];
            _textCost.text = _costNumber.ToString();
        }

        public override void CheckCost(int money, bool _isBuy = false)
        {
            if (_numberOfBoosters > 0 && ButtonBuy != null)
            {
                ButtonBuy.interactable = money >= _costNumber;

                if (_overlayButton != null || _overlayCardBooster != null)
                {
                    if (ButtonBuy.interactable)
                    {
                        if (_overlayButton != null)
                            _overlayButton.SetActive(false);
                        if (_overlayCardBooster != null)
                            _overlayCardBooster.SetActive(false);
                    }

                    else
                    {
                        if (_overlayButton != null)
                            _overlayButton.SetActive(true);
                        if (_overlayCardBooster != null)
                            _overlayCardBooster.SetActive(true);
                        Debug.Log("CHECKCOST " + ButtonBuy.interactable);
                    }
                }
            }
            
            else
            {
                Debug.LogError("Mistake with numberOfBoosters - less 0 value or Button Buy is null");
            }
        }

        public override void Buy()
        {
            BoostsManager.BuyBoost(_asset, _numberOfBoosters);
            Initialize(); // очень важное действие, иначе не будет обновляться результат покупки апдейтов в игре
        }
    }
}