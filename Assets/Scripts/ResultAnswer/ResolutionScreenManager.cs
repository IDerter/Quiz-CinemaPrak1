using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuizCinema
{
    public class ResolutionScreenManager : MonoBehaviour
    {
        [SerializeField] private List<int> _correctAnswersIndex;

        [SerializeField] private GameObject _panelAnswerVar0_2Single;
        [SerializeField] private GameObject _panelAnswerVar0_2SingleInCorrect;

        [SerializeField] private GameObject _panelAnswerVar0_2Multiple;
        [SerializeField] private GameObject _panelAnswerVar0_2MultipleAllCorrect;
        [SerializeField] private GameObject _panelAnswerVar0_2MultipleAllInCorrect;

        [SerializeField] private GameObject _panelAnswerVar3Photo;
        [SerializeField] private GameObject _panelAnswerVar3PhotoInCorrect;

        private Question _currentQuestion;
        [SerializeField]  private List<AnswerData> _currentListAnswerData;

        private void OnEnable()
        {
            _panelAnswerVar0_2Single.SetActive(true);
            _panelAnswerVar0_2SingleInCorrect.SetActive(true);

            _panelAnswerVar0_2Multiple.SetActive(true);
            _panelAnswerVar0_2MultipleAllCorrect.SetActive(true);
            _panelAnswerVar0_2MultipleAllInCorrect.SetActive(true);

            _panelAnswerVar3Photo.SetActive(true);
            _panelAnswerVar3PhotoInCorrect.SetActive(true);
        }

        private void Start()
        {
            AnswersMethods.Instance.OnCreateAnswers += OnCreateAnswers;
            QuestionMethods.Instance.CurrentAnswerList += UpdateCurrentAnswerList;

            GameManager.Instance.OnCorrectAnswer += OnCorrectAnswer;
            GameManager.Instance.OnInCorrectAnswer += OnInCorrectAnswer;
        }

        private void OnDestroy()
        {
            AnswersMethods.Instance.OnCreateAnswers -= OnCreateAnswers;
            QuestionMethods.Instance.CurrentAnswerList += UpdateCurrentAnswerList;

            GameManager.Instance.OnCorrectAnswer -= OnCorrectAnswer;
            GameManager.Instance.OnInCorrectAnswer -= OnInCorrectAnswer;
        }

        private void UpdateCurrentAnswerList(List<AnswerData> list)
        {
            _currentListAnswerData = list;
        }

        private void OnInCorrectAnswer()
        {
            Debug.Log("ONINCORRECTANSWER! " + _currentQuestion._answerType);
            if (_currentQuestion.IndexPrefab <= 2 && _currentQuestion._answerType == AnswerType.Single)
            {
                _panelAnswerVar0_2Single.SetActive(false);
                _panelAnswerVar0_2SingleInCorrect.SetActive(true);
            }

            if (_currentQuestion.IndexPrefab == 3)
            {
                _panelAnswerVar3Photo.SetActive(false);
                _panelAnswerVar3PhotoInCorrect.SetActive(true);
            }

            if (_currentQuestion._answerType == AnswerType.Multiply && _currentListAnswerData.Count == 2 )
            {
                if (_currentListAnswerData[0].AnswerIndex == _correctAnswersIndex[0] || _currentListAnswerData[0].AnswerIndex == _correctAnswersIndex[1] ||
                    _currentListAnswerData[1].AnswerIndex == _correctAnswersIndex[0] || _currentListAnswerData[1].AnswerIndex == _correctAnswersIndex[1])
                {
                    _panelAnswerVar0_2MultipleAllCorrect.SetActive(false);
                    _panelAnswerVar0_2MultipleAllInCorrect.SetActive(false);

                    _panelAnswerVar0_2Multiple.SetActive(true);
                    Debug.Log("ALMOSTCORRECT!");
                }
                else if (_currentQuestion._answerType == AnswerType.Multiply)
                {
                    _panelAnswerVar0_2MultipleAllCorrect.SetActive(false);
                    _panelAnswerVar0_2MultipleAllInCorrect.SetActive(true);
                    Debug.Log("ALLINCORRECT!");
                    _panelAnswerVar0_2Multiple.SetActive(false);
                }
            }

        }

        private void OnCorrectAnswer()
        {

            
            Debug.Log("ONCORRECTANSWER!");
            if (_currentQuestion.IndexPrefab <= 2 && _currentQuestion._answerType == AnswerType.Single)
            {
                _panelAnswerVar0_2Single.SetActive(true);
                _panelAnswerVar0_2SingleInCorrect.SetActive(false);
            }

            if (_currentQuestion.IndexPrefab == 3)
            {
                _panelAnswerVar3Photo.SetActive(true);
                _panelAnswerVar3PhotoInCorrect.SetActive(false);
            }


            if (_currentQuestion._answerType == AnswerType.Multiply)
            {
                _panelAnswerVar0_2MultipleAllCorrect.SetActive(true);
                _panelAnswerVar0_2MultipleAllInCorrect.SetActive(false);
                _panelAnswerVar0_2Multiple.SetActive(false);

            }
        }


        private void OnCreateAnswers(Question question)
        {
            
            _currentQuestion = question;
            Debug.Log("OnCreateAnswers");

            if (GameManager.Instance.CountCurrentAnswer == 0)
                PanelAnswerActivate();
            else
            {
                StartCoroutine(IPanelAnswerActivate());
            }
        }


        IEnumerator IPanelAnswerActivate()
        {
            yield return new WaitForSeconds(GameUtility.ResolutionDelayTime);

            PanelAnswerActivate();
        }

        private void PanelAnswerActivate()
        {
            _correctAnswersIndex = _currentQuestion.GetCorrectAnswers();

            if (_currentQuestion.IndexPrefab < 3 && _currentQuestion._answerType == AnswerType.Single)
            {
                _panelAnswerVar0_2Single.SetActive(true);
                _panelAnswerVar0_2SingleInCorrect.SetActive(true);

                _panelAnswerVar0_2Multiple.SetActive(false);
                _panelAnswerVar0_2MultipleAllCorrect.SetActive(false);
                _panelAnswerVar0_2MultipleAllInCorrect.SetActive(false);

                _panelAnswerVar3Photo.SetActive(false);
                _panelAnswerVar3PhotoInCorrect.SetActive(false);
            }

            else if (_currentQuestion.IndexPrefab < 3 && _currentQuestion._answerType == AnswerType.Multiply)
            {
                _panelAnswerVar0_2Single.SetActive(false);
                _panelAnswerVar0_2SingleInCorrect.SetActive(false);

                _panelAnswerVar0_2Multiple.SetActive(true);
                _panelAnswerVar0_2MultipleAllCorrect.SetActive(true);
                _panelAnswerVar0_2MultipleAllInCorrect.SetActive(true);
                Debug.Log("MultipleSetActive!");
                _panelAnswerVar3Photo.SetActive(false);
                _panelAnswerVar3PhotoInCorrect.SetActive(false);
            }
            else if (_currentQuestion.IndexPrefab == 3 && _currentQuestion._answerType == AnswerType.Single)
            {
                _panelAnswerVar0_2Single.SetActive(false);
                _panelAnswerVar0_2SingleInCorrect.SetActive(false);

                _panelAnswerVar0_2Multiple.SetActive(false);
                _panelAnswerVar0_2MultipleAllCorrect.SetActive(false);
                _panelAnswerVar0_2MultipleAllInCorrect.SetActive(false);

                _panelAnswerVar3Photo.SetActive(true);
                _panelAnswerVar3PhotoInCorrect.SetActive(true);
            }
        }


    }
}