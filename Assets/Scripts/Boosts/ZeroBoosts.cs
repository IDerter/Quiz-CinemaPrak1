using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace QuizCinema
{
    public class ZeroBoosts : MonoBehaviour
    {
        [SerializeField] private GameObject _mainDopBuyBoost;
        [SerializeField] private GameObject _boost;
        [SerializeField] private GameObject _inventoryPanel;

        private const string  _shopSceneName = "Shop";

        public void OpenExtraBuyPanel()
        {
            _mainDopBuyBoost.SetActive(true);
            _boost.SetActive(true);
            _inventoryPanel?.SetActive(false);
        }

        public void CloseExtraBuyPanel()
        {
            _mainDopBuyBoost.SetActive(false);
            _boost.SetActive(false);
            _inventoryPanel?.SetActive(true);
        }
    }
}