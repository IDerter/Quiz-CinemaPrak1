using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QuizCinema
{
    public class UIStars : MonoBehaviour
    {
        [SerializeField] private Image[] _imagesStarsInLvl;
        [SerializeField] private Sprite[] _spritesStars;

        [SerializeField] private GameManager _gameManager;
        [SerializeField] private SpineStarsAnim _spineAnim;

        private void OnEnable()
        {
            _gameManager.OnFinishGame += OnShowStars;
        }

        private void OnShowStars()
        {
            int stars = _gameManager.CalculateLevelStars();
            /*  Debug.Log(stars + " ондявхрюкх йнк-бн гбегд!");

              for (int i = 0; i < stars; i++)
              {
                  _imagesStarsInLvl[i].sprite = _spritesStars[i];
              }
            */
            Debug.Log("stars UISTARS " + $"{stars}");
            _spineAnim.ChangeSkinAnim(stars);
        }

        private void OnDisable()
        {
            _gameManager.OnFinishGame -= OnShowStars;
        }
    }
}