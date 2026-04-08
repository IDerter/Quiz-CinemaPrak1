using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace QuizCinema
{
    public class TextDescription : SingletonBase<TextDescription>
    {
        [SerializeField] private InventoryUIBoosts[] _inventoryUIBoosts;
        [SerializeField] private TextMeshProUGUI _textNoBooster;

        public void ActivateTextDescription(BoostUICount inventoryUIBoosts)
        {
            Debug.Log(inventoryUIBoosts.gameObject.name);

            for (int i = 0; i < _inventoryUIBoosts.Length; i++)
            {
                if (_inventoryUIBoosts[i].name == inventoryUIBoosts.name)
                {
                    _inventoryUIBoosts[i].ShowTextDesription();
                    _textNoBooster?.gameObject.SetActive(false);
                }
                else
                {
                    _inventoryUIBoosts[i].CloseTextDescription();
                }
            }
        }
    }
}