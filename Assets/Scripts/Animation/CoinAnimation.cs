using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

namespace QuizCinema
{
    public class CoinAnimation : MonoBehaviour
    {
        [SerializeField] private GameObject _pileOfCoinParent;
        [SerializeField] private TextMeshProUGUI _counter;
        [SerializeField] private Transform _moveToObject;

        [SerializeField] private Vector3[] _initialPos;
        [SerializeField] private Quaternion[] _intitialRotation;

        private int _countAnimCoins = 7;

        private void Start()
        {
            Debug.Log(_moveToObject.position);

            _initialPos = new Vector3[_countAnimCoins];
            _intitialRotation = new Quaternion[_countAnimCoins];

            for (int i = 0; i < +_pileOfCoinParent.transform.childCount; i++)
            {
                _initialPos[i] = _pileOfCoinParent.transform.GetChild(i).position;
                _intitialRotation[i] = _pileOfCoinParent.transform.GetChild(i).rotation;
            }
        }


        private void Reset()
        {
            for (int i = 0; i < +_pileOfCoinParent.transform.childCount; i++)
            {
                _pileOfCoinParent.transform.GetChild(i).position = _initialPos[i];
                _pileOfCoinParent.transform.GetChild(i).rotation = _intitialRotation[i];
            }
        }

        public void RewardPileOfCoin(int noCoin)
        {
            Reset();
            //var test = RectTransformUtility.WorldToScreenPoint(Camera, Worldpos);

            var delay = 0f;
            _pileOfCoinParent.SetActive(true);

            for (int i = 0; i < _pileOfCoinParent.transform.childCount; i++)
            {
                _pileOfCoinParent.transform.GetChild(i).DOScale(1f, 0.3f).SetDelay(delay).SetEase(Ease.OutBack);

                _pileOfCoinParent.transform.GetChild(i).GetComponent<RectTransform>().DOMove(_moveToObject.position, 0.8f).SetDelay(delay + 0.5f).SetEase(Ease.InBack);

                _pileOfCoinParent.transform.GetChild(i).DORotate(Vector3.zero, 0.5f).SetDelay(delay + 0.5f).SetEase(Ease.Flash);

                _pileOfCoinParent.transform.GetChild(i).DOScale(0f, 0.3f).SetDelay(delay + 1.8f).SetEase(Ease.OutBack);

                delay += 0.1f;
            }
        }
    }
}