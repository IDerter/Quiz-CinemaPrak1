using SpaceShooter;
using System;
using System.Collections;
using System.Collections.Generic;
using TowerDefense;
using UnityEngine;

namespace QuizCinema
{
    public class ActivatePanel : MonoBehaviour
    {
        public static event Action<MapLevel> OnMapLevel;

        [SerializeField] private GameObject _panel;
        [SerializeField] private MapLevel _mapLevel;


        public void OpenPanelStart()
		{
            StartCoroutine(OpenPanel());
		}

        public IEnumerator OpenPanel()
		{
            yield return new WaitForSeconds(LevelSequenceController.Instance.TimeAnimClick);
            _panel.SetActive(true);
            OnMapLevel?.Invoke(_mapLevel);
        }
    }
}