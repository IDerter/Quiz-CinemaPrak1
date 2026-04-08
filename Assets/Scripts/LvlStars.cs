using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TowerDefense;
using System;
using UnityEngine.SceneManagement;

namespace QuizCinema 
{
    public class LvlStars : MapLevel
    {
        public override int Initialize()
        {
            int stars = MapCompletion.Instance.GetLvlStars(gameObject.name);
            
            ResultPanel.SetActive(stars > 0);

            if (ResultImages.Length >= stars)
            {
                for (int i = 0; i < stars; i++)
                {
                    ResultImages[i].sprite = SpritesStarts[i];
                }
            }
            else
            {
                Debug.LogWarning("Error with stars. To much value");
                Debug.Log(stars);
            }
            return stars;
        }
    }
}