using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuizCinema
{
    public enum Languages
    {
        ENG,
        RUS,
        ESP
    }
    public class Language : MonoBehaviour
    {
        [SerializeField] private Languages _language;
        public Languages GetLanguage => _language;



    }
}