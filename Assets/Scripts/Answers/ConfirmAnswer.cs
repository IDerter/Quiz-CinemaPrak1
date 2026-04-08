using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QuizCinema
{
    [RequireComponent(typeof(Button))]
    public class ConfirmAnswer : MonoBehaviour
    {
        public static event Action<AnswerData> OnAcceptAnswer;

        [SerializeField] private Sprite _buttonUnTouchedSprite;
        [SerializeField] private Sprite _buttonTouchedSprite;
        [SerializeField] private Sprite _buttonConfirmSprite;

        [SerializeField] private Image _buttonConfirm;

        [SerializeField] private AnswerData answerData;

        private void Start()
        {
            CadrsAnswers.UpdateButtonCadr += UpdateButtonCadr;
            SwipeDetection.OnSwipeInput += OnSwipeInput;
        }

        private void OnEnable()
        {
            _buttonConfirm = GetComponent<Image>();

            if (_buttonConfirm.TryGetComponent<Button>(out var button))
            {
                button.interactable = false;
            }

            _buttonConfirm.sprite = _buttonUnTouchedSprite;
        }

        private void OnDestroy()
        {
            CadrsAnswers.UpdateButtonCadr -= UpdateButtonCadr;
            SwipeDetection.OnSwipeInput -= OnSwipeInput;
        }


        private void OnSwipeInput(Vector2 arg0)
        {
            _buttonConfirm.sprite = _buttonUnTouchedSprite;

            if (_buttonConfirm.TryGetComponent<Button>(out var button))
            {
                button.interactable = false;
            }
        }

        private void UpdateButtonCadr(AnswerData obj, bool pressed)
        {
            Debug.Log("UpdateButtonCadr" + pressed);
            if (_buttonConfirm.TryGetComponent<Button>(out var button))
            {
                button.interactable = true;
            }
            
            _buttonConfirm.sprite = pressed ? _buttonTouchedSprite : _buttonUnTouchedSprite;


            answerData = obj;

            AcceptAnswer(); // if not check swipe answer
        }

       
       
        public void AcceptAnswer()
        {
            _buttonConfirm.sprite = _buttonConfirmSprite;
            answerData.SwitchCase();

           // OnAcceptAnswer?.Invoke(answerData);
        }
    }
}