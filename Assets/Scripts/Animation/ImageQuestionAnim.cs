using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QuizCinema
{
    public class ImageQuestionAnim : MonoBehaviour
    {
        [SerializeField] private RectTransform _objAnim;
        private Tween objTween;
        [SerializeField] private bool _isLoop;
        public bool IsLoop { get { return _isLoop; } set { _isLoop = value; } }
        [SerializeField] private Transform _startPos;

        private void Awake()
        {

            if (TryGetComponent(out RectTransform rect))
            {
                _objAnim = rect;
                _startPos = rect.transform;
               // Debug.Log(_startPos.position.x + " " + _startPos.position.y);
            }
        }

        [Button()]
        public virtual async void ClickAnim()
        {
            objTween = _objAnim.DOMoveX(transform.position.x + 0.1f, 1f);
            await objTween.ToUniTask();

            objTween = _objAnim.DOMoveX(transform.position.x - 0.1f, 1f);
            await objTween.ToUniTask();
            if (_isLoop)
                ClickAnim();
        }

        public void SetStartPos()
		{
            _objAnim.transform.position = _startPos.transform.position;

        }
    }
}

