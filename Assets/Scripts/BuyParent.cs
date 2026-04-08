using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QuizCinema
{
	public class BuyParent : MonoBehaviour, IBuyUgprade
	{
		[SerializeField] private Button _buttonBuy;
		public Button ButtonBuy {get { return _buttonBuy; } set { _buttonBuy = value; } }

		public virtual void Buy()
		{

		}

		public virtual void CheckCost(int money, bool _isBuy = false)
		{
			
		}

		public virtual void Initialize()
		{

		}
	}
}