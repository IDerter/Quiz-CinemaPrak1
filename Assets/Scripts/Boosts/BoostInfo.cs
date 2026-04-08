using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace QuizCinema
{
    public class BoostInfo : BoostParent
    {
        [SerializeField] private Question _currentQuestion;
        [SerializeField] private GameObject _panelInfoBoost;

        protected override void OnCreateAnswers(Question question)
        {
            _panelInfoBoost.SetActive(false);

            if (_buttonBoost.TryGetComponent<BoostUICount>(out var boost))
            {
                _boostSO = boost.GetSetBoostSO;
            }

           _currentQuestion = question;
            Debug.Log("ЗАХОДИМ В ONCREATEANSWERS В BOOSTInfo!");
           // _buttonBoost.SetActive(true);
            //SwitchInteractable(true, _buttonBoost);
        }

        public override void ActivateBoost(bool everyQuestionActivate)
        {
            //base.ActivateBoost();
            Debug.Log("Активируем буст INFO");


            _panelInfoBoost.SetActive(true);
            var descriptionFilm = _panelInfoBoost.GetComponentInChildren<TextMeshProUGUI>();
            if (descriptionFilm.TryGetComponent<TextMeshProUGUI>(out var text))
            {
                descriptionFilm.text = _currentQuestion.ListDescriptionFilm[PlayerPrefs.GetInt("IndexLanguageSave")].ToString();
            }
            Debug.Log(_currentQuestion.ListDescriptionFilm[PlayerPrefs.GetInt("IndexLanguageSave")].ToString());
            SwitchInteractable(false, _buttonBoost);

            if (!everyQuestionActivate)
                BoostsManager.UseBoost(_boostSO);
        }

       

        public void CloseQuestionInfo()
        {
            _panelInfoBoost.SetActive(false);
        }
    }
}