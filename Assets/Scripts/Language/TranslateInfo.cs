using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuizCinema
{
    public class TranslateInfo : SingletonBase<TranslateInfo>
    {
        [SerializeField] private List<string> _textCorrect = new List<string> { "is correct", "правильный"};
        public List<string> TextCorrect => _textCorrect;

        [SerializeField] private List<string> _textInCorrect = new List<string> { "is wrong", "неправильный" };
        public List<string> TextInCorrect => _textInCorrect;

        [SerializeField] private List<string> _textAnswer = new List<string> { "Answer", "Ответ" };
        public List<string> TextAnswer => _textAnswer;

        [SerializeField] private List<string> _textProfile = new List<string> { "Profile", "Профиль" };
        public List<string> TextProfile => _textProfile;

        [SerializeField] private List<string> _textMap = new List<string> { "Map", "Карта" };
        public List<string> TextMap => _textMap;

        [SerializeField] private List<string> _textBattle = new List<string> { "Battle", "Баттл" };
        public List<string> TextBattle => _textBattle;

        [SerializeField] private List<string> _textShop = new List<string> { "Shop", "Магазин" };
        public List<string> TextShop => _textShop;
    }
}