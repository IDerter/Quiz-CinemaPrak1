using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace QuizCinema
{
	public class BuySkin : BuyParent
	{
		[SerializeField] private SkinSO _asset;
		public SkinSO SkinSO { get { return _asset; } set { _asset = value; } }
		[SerializeField] private Image _upgradeIcon;
		[SerializeField] private TextMeshProUGUI _textCount;
		[SerializeField] private TextMeshProUGUI _textCost;

		[SerializeField] private GameObject _overlayButton;
		[SerializeField] private GameObject _overlayCardSkin;

		[SerializeField] private Button _buttonBuySkin;
		[SerializeField] private GameObject _viewSelect;

		private int _costSkin; // цена покупки улучшения

		private void Awake()
		{
			Initialize();
			ButtonBuy = _buttonBuySkin;
		}

		private void Start()
		{
			PurchaseManager.PurchaseOn += PurchaseOn;
		}

		private void OnDestroy()
		{
			PurchaseManager.PurchaseOn -= PurchaseOn;
		}

		private void PurchaseOn(string type)
		{
			Debug.Log(type);
			Debug.Log(TypeReward.Thief.ToString() + " " +  _asset.name);
			if (type == TypeReward.Thief.ToString() && _asset.IsPurshase && _asset.name == type)
			{
				Buy();
			}
		}

		public override void Initialize()
		{
			//Debug.Log(_asset.name.ToString());
			if (_upgradeIcon != null)
				_upgradeIcon.sprite = _asset.sprite;
	
			_costSkin = _asset.cost;

			if (_textCost != null)
			{
				_textCost.text = _asset.cost.ToString();
				Debug.Log("Initiliaze" + " " + _textCost);
			}
		}

		public override void CheckCost(int money, bool _isBuy = false)
		{
			ButtonBuy.interactable = money >= _costSkin || _asset.IsPurshase;

			if (_overlayButton != null && _overlayCardSkin != null)
			{
				if (ButtonBuy.interactable || _isBuy)
				{
					_overlayButton.SetActive(false);
					_overlayCardSkin.SetActive(false);
				}
						
				else
				{
					_overlayButton.SetActive(true);
					_overlayCardSkin.SetActive(true);
					Debug.Log("CHECKCOST " + ButtonBuy.interactable);
				}
			}
		}

		public override void Buy()
		{
			Debug.Log("BuySkin");
			SkinManager.BuySkin(_asset);
			Initialize(); // очень важное действие, иначе не будет обновляться результат покупки апдейтов в игре
		}

		public void PutOn()
		{
			SkinManager.PutOnSkin(_asset);
			if (_viewSelect != null)
				_viewSelect.SetActive(true);
		}

		public void TakeOff()
		{
			SkinManager.TakeOffSkin(_asset);
		}
	}
}