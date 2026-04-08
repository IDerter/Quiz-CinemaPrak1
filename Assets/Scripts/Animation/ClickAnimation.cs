using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuizCinema
{
    [RequireComponent (typeof(RectTransform))]
    public class ClickAnimation : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private float _timeDecreaseSize = 0.002f;
        private float _step = 1f;
        private float _valueOnSizeChange = 50f;


        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void StartAnim()
        {
            StartCoroutine(AnimClick());
        }

        IEnumerator AnimClick()
        {

            var rectsize = _rectTransform.sizeDelta;
            // _timerText.color = _timerDefaultColor;

            while (rectsize.x < _rectTransform.sizeDelta.x + _valueOnSizeChange)
            {
                _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x - _step, _rectTransform.sizeDelta.y - _step);

                yield return new WaitForSeconds(_timeDecreaseSize);
            }

            while (rectsize.x > _rectTransform.sizeDelta.x)
            {
                _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x + _step, _rectTransform.sizeDelta.y + _step);

                yield return new WaitForSeconds(_timeDecreaseSize * 2);
            }
        }
    }
}