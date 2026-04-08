using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace QuizCinema
{
    public enum AnswerType { Multiply, Single }

    [Serializable()]
    public class Answer
    {
        public List<string> InfoList;

        public string Info;

        public string TranslateInfo;

        public bool IsCorrect;

        public Answer() { }
    }

    [Serializable()]
    public class Question
    {
        /// <summary>
        /// Index 0-2 - text questions
        /// Index = 3 - image questionsd
        /// </summary>
        public int IndexPrefab = 0;
        public AnswerType _answerType = AnswerType.Single;
        public AnswerType GetAnswerType => _answerType;

        public List<string> ListInfoQuestion;

        public string Info = String.Empty;

        public List<string> ListNoteFilm;

        public string NoteFilm = String.Empty;

        public List<string> ListDescriptionFilm;

        public string Director = String.Empty;

        public string Poster = String.Empty;

        public string _cadrCinemaName;
        public string _cadrCinemaNameTranslateRu;

        public Answer[] Answers = null;

        // Parameters
        public bool UseTimer = false;

        public int Timer = 0;


        public int AddScore = 0;

        public Question() { }


        public List<int> GetCorrectAnswers()
        {
            Debug.Log("GetCorrectAnswers");
            
            List<int> correctAnswers = new List<int>();
            for (int i = 0; i < Answers.Length; i++)
            {
                if (Answers[i].IsCorrect)
                {
                    correctAnswers.Add(i);
                }
            }
            return correctAnswers;
        } 
    }
}