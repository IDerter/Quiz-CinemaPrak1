using UnityEngine.UI;
using UnityEngine;
using System;

namespace TowerDefense
{
    public class BuyUpgrade : MonoBehaviour
    {
        [SerializeField] private UpgradeAsset _asset;
        [SerializeField] private Image _upgradeIcon;
        [SerializeField] private Text _textLvl;
        [SerializeField] private Text _textCost;
        [SerializeField] private Button _buttonBuy;
        public Button GetButton => _buttonBuy;

        [SerializeField] private GameObject[] _buttonsGameobjects;

        private int _costNumber; // цена покупки улучшения

        public void Initialize()
        {
            _upgradeIcon.sprite = _asset.sprite;
            var savedLevel = Upgrades.GetUpgradeLevel(_asset);
            _textLvl.text = $"Lvl: {savedLevel + 1}";
            if (savedLevel >= _asset.costByLevel.Length)
            {
                _textLvl.text += "(Max)";
                _buttonBuy.interactable = false;
                foreach (var i in _buttonsGameobjects)
                {
                    i.SetActive(false);
                }
                _textCost.text = "X";
                _costNumber = int.MaxValue;
            }
            else
            {
                _costNumber = _asset.costByLevel[savedLevel];
                _textCost.text = _costNumber.ToString();
            }
            Debug.Log("Initiliaze");
        }

        public void CheckCost(int money)
        {
            _buttonBuy.interactable = money >= _costNumber;
        }

        public void Buy()
        {
            Upgrades.BuyUpgrade(_asset);
            Initialize(); // очень важное действие, иначе не будет обновляться результат покупки апдейтов в игре
        }
    }
}