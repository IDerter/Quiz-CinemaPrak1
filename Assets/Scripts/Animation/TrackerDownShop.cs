using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;

using TMPro;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using System;

namespace QuizCinema
{
    public class TrackerDownShop : MonoBehaviour
    {
        public static event Action OnScrollbarStart;
        public static event Action OnScrollbarEnd;

        [SerializeField] private GameObject _panelToMove;
        [SerializeField] private Transform _posUp;
        [SerializeField] private Transform _posDown;

        [SerializeField] private float duration = 1f;
        private const string _clickSwipeAudio = "ClickSwipe";

        public async void MovePanelUp()
		{
            var task = _panelToMove.transform.DOMove(new Vector3(_posUp.position.x, _posUp.transform.position.y, 0), duration)
                .SetEase(Ease.OutBack).ToUniTask();

            AudioManager.Instance.PlaySound(_clickSwipeAudio);

            await UniTask.WhenAll(task);
            OnScrollbarStart?.Invoke();
        }

        public void MovePanelDown()
        {
            var task = _panelToMove.transform.DOMove(new Vector3(_posDown.position.x, _posDown.transform.position.y, 0), duration)
                .SetEase(Ease.InBack).ToUniTask();

            AudioManager.Instance.PlaySound(_clickSwipeAudio);

            OnScrollbarEnd?.Invoke();
        }
    }
}