using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static QuizCinema.DataLikeCinema;

namespace QuizCinema
{
    public class LikeCinemaPanel : MonoBehaviour
    {
        [SerializeField] private Image _posterImage;
        [SerializeField] private TextMeshProUGUI _textCinemaName;
        [SerializeField] private TextMeshProUGUI _textCinemaInfo;
        [SerializeField] private CinemaInfo _cinemaInfo;
        public Image PosterImage { get { return _posterImage; } set { _posterImage = value; } }
        public TextMeshProUGUI TextCinemaName { get { return _textCinemaName; } set { _textCinemaName = value; } }
        public TextMeshProUGUI TextCinemaInfo { get { return _textCinemaInfo; } set { _textCinemaInfo = value; } }
        public CinemaInfo CinemaInfo { get { return _cinemaInfo; } set { _cinemaInfo = value; } }

        public void DeleteFilmFromPanelLike()
		{
            ResetCurrentCinema(_cinemaInfo.Question);
            Destroy(gameObject);
		}
    }
}