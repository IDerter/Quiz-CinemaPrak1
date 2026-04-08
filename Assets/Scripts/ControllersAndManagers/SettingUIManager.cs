using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace QuizCinema
{
    [CreateAssetMenu]
    public class SettingUIManager : ScriptableObject
    {
        [SerializeField] private AnswerData[] _answerPrefab;
        public AnswerData[] AnswersPrefabs => _answerPrefab;

        [SerializeField] private UIElements _uIElements;
        public UIElements UIGameElements => _uIElements;


        /* [SerializeField] private string _episodeName;
         public string EpisodeName => _episodeName;

         [SerializeField] private string[] _levels;

         public string[] Levels => _levels;

         [SerializeField] private Sprite _previewImage;

         public Sprite PreviewImage => _previewImage;
        */
    }
}