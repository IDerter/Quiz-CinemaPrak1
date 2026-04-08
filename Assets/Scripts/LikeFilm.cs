using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QuizCinema
{
    public class LikeFilm : SingletonBase<LikeFilm>
    {
        [SerializeField] private Sprite _likeSpritePress;
        public Sprite GetLikeSpritePress => _likeSpritePress;

        [SerializeField] private Sprite _likeSpriteUnPress;
        public Sprite GetLikeSpriteUnPress => _likeSpriteUnPress;

        public static void SetDefaultValue(Image likeImage)
        {
            likeImage.sprite = Instance._likeSpriteUnPress;
        }

        public static void PressButtonLike(Image likeImage, Question _currentQuestion, bool flagLike)
        {
            //flagLike = !flagLike;

            if (flagLike)
            {
                likeImage.sprite = Instance._likeSpritePress;
                DataLikeCinema.SaveCinema(_currentQuestion);
            }
            else
            {
                likeImage.sprite = Instance._likeSpriteUnPress;
                DataLikeCinema.ResetCurrentCinema(_currentQuestion);
            }
        }

        public static void PressButtonLikeMultiple(Image likeImage, Question _currentQuestion, string cadrName, string description, bool flagLike)
        {

            if (flagLike)
            {
                likeImage.sprite = Instance._likeSpritePress;
                DataLikeCinema.SaveCinema(_currentQuestion, cadrName, description);
            }
            else
            {
                likeImage.sprite = Instance._likeSpriteUnPress;
                DataLikeCinema.ResetCurrentCinema(_currentQuestion, cadrName);
            }
        }
    }
}