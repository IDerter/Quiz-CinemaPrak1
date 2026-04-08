using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuizCinema
{

    public class SpineStarsAnim : MonoBehaviour
    {
        [SerializeField] private SkeletonGraphic _skeletonStars;
        [SerializeField] private SkeletonGraphic _skeletonRays;


        public void ChangeSkinAnim(int countStars)
        {
            switch (countStars)
            {
                case 1:
                    _skeletonStars.Skeleton.SetSkin("1 star");
                    break;
                case 2:
                    _skeletonStars.Skeleton.SetSkin("2 stars");
                    break;
                case 3:
                    _skeletonStars.Skeleton.SetSkin("3 stars");
                    break;
                default:
                    _skeletonStars.Skeleton.SetSkin("default");
                    break;
            }

            Debug.Log(_skeletonStars.Skeleton.Skin);
            _skeletonStars.AnimationState.SetAnimation(1, "anima_stars_level_complete", false);
            _skeletonStars.freeze = false;
            _skeletonRays.freeze = false;
        }
    }
}