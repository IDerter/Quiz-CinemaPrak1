using TMPro;
using TowerDefense;
using UnityEngine;

namespace QuizCinema
{
	public class BoostShop : MonoBehaviour
    {
        [SerializeField] private int _money;
        [SerializeField] private TextMeshProUGUI _textMoney;
        [SerializeField] private BuyParent[] _sales;

        private void Start()
        {
            foreach (var slot in _sales)
            {
                slot.Initialize();
                slot.ButtonBuy.onClick.AddListener(UpdateMoney);
                //Debug.Log(slot.ButtonBuy.name);
            }
            UpdateMoney();
        }

        private void OnDestroy()
        {
            foreach (var slot in _sales)
            {
                slot.ButtonBuy.onClick.RemoveListener(UpdateMoney);
            }
        }

        public void UpdateMoney()
        {
            _money = (MapCompletion.Instance.TotalScoreLvls + MapCompletion.Instance.TotalAdsMoney) - MapCompletion.Instance.MoneyShop - MapCompletion.Instance.SkinShop;


            foreach (var slot in _sales)
            {
                if (slot.TryGetComponent(out BuySkin buySkin))
                {
                    slot.CheckCost(_money, SkinManager.CheckIsBuySkin(buySkin.SkinSO));
                    
                }
                else
                {
                    slot.CheckCost(_money, false);
                }
            }
        }

        public void Buy(BoostSO boostAsset)
        {
            //BoostsManager.BuyBoost(boostAsset, _sales);
        }
    }
}