using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuizCinema
{
    public class DopBuy : MonoBehaviour
    {
        [SerializeField] private GameObject[] _panelBuyBoosters;
        [SerializeField] private GameObject _buyPanel;
        [SerializeField] private GameObject _panelSizeInventory;

        public void CloseBuyPanel()
        {
            _buyPanel.SetActive(false);

            foreach (var panel in _panelBuyBoosters)
            {
                panel.SetActive(false);
            }
            _panelSizeInventory.SetActive(true);
        }
    }
}