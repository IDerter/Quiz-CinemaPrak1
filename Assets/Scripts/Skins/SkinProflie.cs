using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static QuizCinema.SkinManager;

namespace QuizCinema
{
    public class SkinProflie : MonoBehaviour
    {
		[SerializeField] private SkeletonGraphic _profileData;

		private void Start()
		{
			SkinManager.Instance.OnPutOn += OnPutOn;
		}

		private void OnDestroy()
		{
			SkinManager.Instance.OnPutOn -= OnPutOn;
		}

		private void OnPutOn(SkinSave skinSave)
		{
			//var skin = GetPutOnSkin();
			//Debug.Log(skin.SkinName);
			//_profileData.Skeleton.SetSkin(skin.SkinName);
		}
	}
}