using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace QuizCinema
{
    public class BoostUICount : MonoBehaviour
    {
        // ”¡–¿“‹ À»ÿÕﬁﬁ ÀŒ√» ” » œ≈–≈Õ≈—“» ¬ ƒŒ◊≈–Õ»≈  À¿——€!
        [SerializeField] protected BoostSO _boostSO;
        public BoostSO GetSetBoostSO { get { return _boostSO; } set { _boostSO = value; } }
        [SerializeField] protected Image _boostImage;
        public Image BoostImage { get { return _boostImage; } set { _boostImage = value; } }



        protected const string _shopSceneName = "Shop";
        private Question _question;


        private void OnEnable()
        {
            BoostsManager.OnPressButtonBoost += OnPressButtonBoost;
        }

       // protected virtual void OnCreateAnswers(Question obj)
    //    {
       //     CheckButtonInteractable();
      //      _question = obj;
     //   }

        protected virtual void OnPressButtonBoost()
        {
           // _buttonBoost.interactable = false;

            CheckButtonInteractable();
        }

        private void OnDestroy()
        {
            BoostsManager.OnPressButtonBoost -= OnPressButtonBoost;
        }

     

        private void Start()
        {
           // _buttonBoost = GetComponent<Button>();

            CheckButtonInteractable();
        }

        protected virtual void CheckButtonInteractable()
        {


        }

    }
}