using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QuizCinema
{
    public class LockAnim : MonoBehaviour
    {
        [SerializeField] private SkeletonGraphic _lock;
        [SerializeField] private BarAnim _barAnim;
        [SerializeField] private Button _buttonLock; 

        private string _namePress = "press";

		private void Start()
		{
            _buttonLock.onClick.AddListener(() => LockBar());
        }


		public void LockBar()
		{
            if (!_barAnim.IsOpen)
			{
                _lock.AnimationState.SetAnimation(1, _namePress, false);
                AudioManager.Instance.PlaySound("Lock");
            }
			else
			{
                _buttonLock.interactable = false;
			}
        }
    }
}