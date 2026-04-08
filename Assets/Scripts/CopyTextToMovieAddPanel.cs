using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace QuizCinema
{
    public class CopyTextToMovieAddPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textCinemaName;
        [SerializeField] private TextMeshProUGUI _textCinemaInfo;

        


        private void Start()
        {
            AnswersMethods.Instance.OnCreateAnswers += OnCreateAnswers;
            AnswersMethods.Instance.OnCorrectAnswer += OnCorrectAnswers;
        }

        private void OnDestroy()
        {
            AnswersMethods.Instance.OnCreateAnswers -= OnCreateAnswers;
            AnswersMethods.Instance.OnCorrectAnswer -= OnCorrectAnswers;
        }

        private void OnCorrectAnswers(List<AnswerData> answer)
        {
            // TODO //
            _textCinemaName.text = answer[0].InfoText.text;
        }

        private void OnCreateAnswers(Question question)
        {
            _textCinemaInfo.text = question.ListDescriptionFilm[PlayerPrefs.GetInt("IndexLanguageSave")];
        }
    }
}