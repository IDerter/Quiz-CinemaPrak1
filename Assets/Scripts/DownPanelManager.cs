using SpaceShooter;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace QuizCinema
{
    public class DownPanelManager : MonoBehaviour
    {
        [SerializeField] private Button _buttonProfile;
        [SerializeField] private Button _buttonMap;
        [SerializeField] private Button _buttonShop;
        [SerializeField] private Button _buttonBattle;

        //[SerializeField] private bool _
        [SerializeField] private DownPanelAnim _currentAnimButton;
        [SerializeField] private const string _modeScene = "ModeScene";



        private void Start()
        {
            _buttonProfile.onClick.AddListener(() => AnimButton(_buttonProfile.GetComponent<DownPanelAnim>()));
            _buttonMap.onClick.AddListener(() => AnimButton(_buttonMap.GetComponent<DownPanelAnim>()));
            _buttonShop.onClick.AddListener(() => AnimButton(_buttonShop.GetComponent<DownPanelAnim>()));

            if (SceneManager.GetActiveScene().name == _modeScene)
			{
                Debug.Log("DownPanelManager StartCoroutine");
                StartCoroutine(LoadAnimWithDelay());
			}
               
            //_buttonBattle.onClick.AddListener(() => MoveAndScaleButton(_buttonBoosters));
        }

        public void AnimButton(DownPanelAnim button)
        {
            if (!button.IsSelect)
            {
                DisableAnim();

                if (_buttonMap.GetComponent<DownPanelAnim>() != button || SceneManager.GetActiveScene().name == _modeScene)
				{
                    button.ActivateAnim(button.GetGraphicButton);
                    _currentAnimButton = button;
                }
                else
				{
                    if (SceneManager.GetActiveScene().name == _modeScene)
					{
                        button.ActivateAnim(button.GetGraphicButton);
                        _currentAnimButton = button;
                    }
				}
            }
        }

        public void DisableAnim()
        {
            if (_currentAnimButton != null && _currentAnimButton.IsSelect)
                _currentAnimButton.DisableAnim(_currentAnimButton.GetGraphicButton);
        }

        private IEnumerator LoadAnimWithDelay()
        {
            yield return new WaitForSeconds(LevelSequenceController.Instance.TimeAnimClick);

            AnimButton(_buttonMap.GetComponent<DownPanelAnim>());
        }
    }
}