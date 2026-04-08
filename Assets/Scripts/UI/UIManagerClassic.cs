using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TowerDefense;
using SpaceShooter;
using TMPro;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.Networking;
using System.IO;

namespace QuizCinema
{
    public class UIManagerClassic : UIManager
    {

        protected override void DisplayResolution(ResolutionScreenType type, int score)
        {
            Debug.Log($"DisplayResolution {type.ToString()}");

            UpdateResUI(type, score);
            _uIElements.ResolutionScreenAnimator.SetInteger(_resStateParaHash, 2);
            _typeAnswer = type;

            Debug.Log("CloseResultQuestion");
            CloseResultQuestion();
        }


        protected override void UpdateResUI(ResolutionScreenType type, int score)
        {
            _uIElements.NumberCurrentQuestion.text = _gameManager.CountCorrectAnswer.ToString(); // + "/" + _questionMethods.Data.Questions.Length;

            var maxScoreCurrentLvl = MapCompletion.Instance.GetMaxCountClassicRegime();
            Debug.Log(maxScoreCurrentLvl);

            if (_uIElements.MaxScoreFinalLvl.Length == 2)
            {
                _uIElements.MaxScoreFinalLvl[0].text = maxScoreCurrentLvl.ToString();
                _uIElements.MaxScoreFinalLvl[1].text = maxScoreCurrentLvl.ToString();
            }
        }


        protected override void UpdateFinishScreen()
        {
            Debug.Log("UpdateFinishScreenClassic");
            var sceneName = SceneManager.GetActiveScene().name;

            if (_uIElements.CountCorrectAnswer.Length == 2)
            {
                _uIElements.CountCorrectAnswer[0].text = _gameManager.CountCorrectAnswer + "/40";
                _uIElements.CountCorrectAnswer[1].text = _gameManager.CountCorrectAnswer + "/40";
            }
            else
            {
                Debug.LogError("Fill in the field CountCorrectAnswer");
            }
        }
        
        public override void StartCalculateScore(int levelCountsStars)
        {
            Debug.Log("StartCalculateScore " + MapCompletion.Instance.LearnSteps[1]);
            CalculateScore(levelCountsStars);
        }

        public override void CalculateScore(int levelCountsStars)
        {
            Debug.Log("StartCalculate with DOTween");

            if (_gameManager.CountCurrentAnswer == 0)
            {
                if (_uIElements.ScoreFinalLvl.Length == 2)
                {
                    _uIElements.ScoreFinalLvl[0].text = "0";
                    _uIElements.ScoreFinalLvl[1].text = "0";
                }
            }

           // MapCompletion.SaveEpisodeResult(levelCountsStars, _score.CurrentLvlScore);

            var maxScoreCurrentLvl = MapCompletion.Instance.GetMaxCountClassicRegime();
            Debug.Log(maxScoreCurrentLvl);

            if (_uIElements.MaxScoreFinalLvl.Length == 2)
            {
                _uIElements.MaxScoreFinalLvl[0].text = maxScoreCurrentLvl.ToString();
                _uIElements.MaxScoreFinalLvl[1].text = maxScoreCurrentLvl.ToString();
            }

            _isCalculating = true;

            int startScore = 0;
            int targetScore = _score.CurrentLvlScore;
            float animationDuration = 1.5f;

            DOTween.To(() => startScore, x => startScore = x, targetScore, animationDuration)
                .SetEase(Ease.OutQuad)
                .OnUpdate(() =>
                {
                    if (_uIElements.ScoreFinalLvl.Length == 2)
                    {
                        _uIElements.ScoreFinalLvl[0].text = startScore.ToString();
                        _uIElements.ScoreFinalLvl[1].text = startScore.ToString();
                    }
                    if ( _uIElements.MaxScoreFinalLvl.Length == 2)
                    {
                        _uIElements.MaxScoreFinalLvl[0].text = startScore.ToString();
                        _uIElements.MaxScoreFinalLvl[1].text = startScore.ToString();
                    }
                })
                .OnComplete(() =>
                {
                    _uIElements.ScoreFinalLvl[0].text = targetScore.ToString();
                    _uIElements.ScoreFinalLvl[1].text = targetScore.ToString();

                    _uIElements.MaxScoreFinalLvl[0].text = targetScore.ToString();
                    _uIElements.MaxScoreFinalLvl[1].text = targetScore.ToString();
                    

                    FinishScoreCalculating();
                    _isCalculating = false;
                    Debug.Log("Score calculation finished.");
                });
        }

        protected override void UpdateScoreUI()
        {
            Debug.Log("UpdateScore UI Classic");

            if (_gameManager.CountCorrectAnswer <= _questionMethods.Data.Questions.Length)
                _uIElements.NumberCurrentQuestion.text = _gameManager.CountCorrectAnswer.ToString(); // + "/" + _questionMethods.Data.Questions.Length;

            if (_uIElements.ScoreFinalLvl.Length == 2)
            {
                _uIElements.ScoreFinalLvl[0].text = _gameManager.CountCorrectAnswer.ToString();
                _uIElements.ScoreFinalLvl[1].text = _gameManager.CountCorrectAnswer.ToString();
            }

            var maxScoreCurrentLvl = MapCompletion.Instance.GetMaxCountClassicRegime();
            Debug.Log(maxScoreCurrentLvl);

            if (_uIElements.MaxScoreFinalLvl.Length == 2)
            {
                _uIElements.MaxScoreFinalLvl[0].text = maxScoreCurrentLvl.ToString();
                _uIElements.MaxScoreFinalLvl[1].text = maxScoreCurrentLvl.ToString();
            }

            QuestionMethodsClassic.Instance.IsFinishedClassic = false;
        }
    }
}