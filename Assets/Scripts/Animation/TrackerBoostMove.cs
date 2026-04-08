using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;

using TMPro;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

namespace QuizCinema
{
    public class TrackerBoostMove : MonoBehaviour
    {
		[SerializeField] private BoostInventory _boostInventory;
		[SerializeField] private InventoryUIBoosts[] _inventoryUIBoosts;

		/*
		private void Start()
		{
			_boostInventory.OnPressButtonBoostInventory += OnPressButtonBoostInventory;

			foreach (var boost in _inventoryUIBoosts)
			{
				if (boost.GetButtonBoost.TryGetComponent<Button>(out Button boostButton))
				{
					boostButton.onClick.AddListener(() => MoveToButton(boostButton));
					Debug.Log("TracketBoostMove" + " " + boostButton.name + " " + boost.name);
				}
			}
		}

		private void OnDestroy()
		{
			_boostInventory.OnPressButtonBoostInventory -= OnPressButtonBoostInventory;
		}

		private void OnPressButtonBoostInventory(BoostUICount obj)
		{
			Debug.Log(obj.name);
		}

		public void MoveToButton(Button button)
		{
			Transform parentTransform = button.transform.parent;
			Debug.Log("MoveToButton  " + parentTransform.name);

			for (int i = 0; i < _boostInventory.ListBoosts.Length; i++)
			{
				if (_boostInventory.ListBoosts[i] == null)
				{
					var obj = Instantiate(parentTransform.gameObject, parentTransform);
					Transform transformToMove = _boostInventory.ListInventoryBoosts[i].transform;

					obj.transform.DOMove(new Vector3(transformToMove.GetComponent<RectTransform>().position.x, transformToMove.transform.position.y), 2f)
						.SetEase(Ease.Linear).ToUniTask();
					break;
				}
			}


			/*_chooseImage.transform.DOMove(new Vector3(button.GetComponent<RectTransform>().position.x, _chooseImage.transform.position.y), duration)
				.SetEase(Ease.Linear).ToUniTask();

			_chooseImage.transform.DOScale(new Vector3(button.GetComponent<RectTransform>().localScale.x, button.GetComponent<RectTransform>().localScale.y), duration)
				.SetEase(Ease.Linear).ToUniTask();
			

			//button.GetComponent<UpTextShop>().ActivateText();
		}
		*/
	}
}