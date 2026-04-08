using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace TowerDefense {
    public class UpgradeShop : MonoBehaviour
    {
        [SerializeField] private int _money;
        [SerializeField] private TextMeshProUGUI _textMoney;
        [SerializeField] private BuyUpgrade[] _sales;

        private void Start()
        {
            foreach (var slot in _sales)
            {
                slot.Initialize();
                slot.GetButton.onClick.AddListener(UpdateMoney);
            }
            UpdateMoney();
        }

        public void UpdateMoney()
        {
            print("UpdateUPGRADESHOP");
            _money = MapCompletion.Instance.TotalScoreLvls;
            _money -= Upgrades.GetTotalCostUpgrade();
            MapCompletion.Instance.MoneyShop = Upgrades.GetTotalCostUpgrade();

            _textMoney.text = _money.ToString();
            foreach(var slot in _sales)
            {
                slot.CheckCost(_money);
            }
        }

        public void Buy(UpgradeAsset upgradeAsset)
        {
            Upgrades.BuyUpgrade(upgradeAsset);
        }
    }
}