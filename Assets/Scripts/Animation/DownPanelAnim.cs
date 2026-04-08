using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace QuizCinema
{
    public class DownPanelAnim : MonoBehaviour
    {
        [SerializeField] private SkeletonGraphic _graphicAnimButton;
        public SkeletonGraphic GetGraphicButton => _graphicAnimButton;

        [SerializeField] private string _nameAnimDeselect;
        [SerializeField] private string _nameAnimSelect;

        [SerializeField] private bool _isSelect = true;
        public bool IsSelect => _isSelect;
        [SerializeField] private bool _isLoadLevel;
        public bool IsLoadLevel => _isLoadLevel;
        [SerializeField] private LevelMapButtonController _buttonController;

        private void Start()
        {
            var animSpineArray = _graphicAnimButton.Skeleton.Data.Animations.ToArray();
            _nameAnimDeselect = animSpineArray[0].ToString();
            _nameAnimSelect = animSpineArray[1].ToString();
        }


        public void ActivateAnim(SkeletonGraphic graphic)
        {
            _isSelect = true;

            if (_isLoadLevel)
			{
                if (_buttonController != null)
				{
                    _buttonController.LoadLevelMap();
				}
			}
			else
			{
                graphic.AnimationState.SetAnimation(1, _nameAnimSelect, false);
            }
        }

        public void DisableAnim(SkeletonGraphic graphic)
        {
            if (_isSelect)
			{
                _isSelect = false;
                graphic.AnimationState.SetAnimation(1, _nameAnimDeselect, false);
            }
        }
    }
}