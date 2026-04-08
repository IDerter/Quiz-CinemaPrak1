using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace QuizCinema
{
    public class ManagerDownPanel : SingletonBase<ManagerDownPanel>
    {
        [SerializeField] private const string _sceneMap = "LevelMap";
        [SerializeField] private const string _sceneProfile = "Profile";

        [SerializeField] private SkeletonGraphic _profileGraphic;
        [SerializeField] private Image _overlaySelectProfile;


        [SerializeField] private SkeletonGraphic _mapGraphic;
        [SerializeField] private Image _overlaySelectMap;

        private void Start()
        {
            Debug.Log(SceneManager.GetActiveScene().name);
        /*    switch (SceneManager.GetActiveScene().name)
            {
                case _sceneMap:
                    Debug.Log("CURRENT SCENE IS LVLMAP");
                    DownPanelAnim.Instance.ActivateAnim(_mapGraphic, _overlaySelectMap, true);
                    if (DownPanelAnim.Instance.GetSceneLastAnim == _sceneProfile)
                        DownPanelAnim.Instance.DisableAnim(_profileGraphic, _overlaySelectProfile);

                    break;
                case _sceneProfile:
                    Debug.Log("CURRENT SCENE IS Profile");
                    DownPanelAnim.Instance.ActivateAnim(_profileGraphic, _overlaySelectProfile, true);
                    if (DownPanelAnim.Instance.GetSceneLastAnim == _sceneMap)
                        DownPanelAnim.Instance.DisableAnim(_mapGraphic, _overlaySelectMap);
                    break;

                default:
                    break;
            }
        */
        }


    }
}