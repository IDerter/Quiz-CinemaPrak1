using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using DG.Tweening;

using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
using TowerDefense;
using UnityEngine.UI;

namespace QuizCinema
{
    public class KeyNewBarAnim : MonoBehaviour
    {
        [SerializeField] private GameObject keyPrefab;

        [SerializeField] private Transform keyParent;

        [SerializeField] private Transform spawnLocation;

        [SerializeField] private Transform endPosition;
        [SerializeField] private Transform endPositionToPunch;

        [SerializeField] private float duration;

        [SerializeField] private float minX;

        [SerializeField] private float maxX;

        [SerializeField] private float minY;

        [SerializeField] private float maxY;

        private GameObject key;

        private Tween keyReactionTween;

        [SerializeField] private int coinStart;
        [SerializeField] private int coinEnd;
        [SerializeField] private int needToSum;
        [SerializeField] private GameObject _starObj;
        [SerializeField] private BarSliderProgress _sliderProgress;

        private void Start()
        {
            KeyAnimStart();
        }

        public void ShowIntersitialAdAfterOpenNextBar()
		{
            AdsManager.Instance._interstitialAds.ShowInterstitialAd();
        }


        [Button()]
        public async void KeyAnimStart()
        {
            Debug.Log("StartKeyAnim");
            GameObject keyInstance = Instantiate(keyPrefab, keyParent);
            float xPosition = spawnLocation.position.x + Random.Range(minX, maxX);
            float yPosition = spawnLocation.position.y + Random.Range(minY, maxY);

            keyInstance.transform.position = new Vector3(xPosition, yPosition);
           // await keyInstance.transform.DOPunchPosition(new Vector3(0, 30, 0), Random.Range(0, 1f)).SetEase(Ease.InOutElastic)
            //    .ToUniTask();
            key = keyInstance;

            await key.transform.GetComponent<Image>().DOFade(1, duration);
            await UniTask.Delay(TimeSpan.FromSeconds(0.01f));

            Destroy(_sliderProgress.gameObject);
            Destroy(_starObj);
            MoveKeyTask();
            // Animation the reaction when collecting coin
        }


        private void MoveKeyTask()
        {
            UniTask moveKeyTask = new UniTask();
            moveKeyTask = MoveKeyTask(key);
            
        }

        private async UniTask MoveKeyTask(GameObject keyInstance)
        {
            Debug.Log("MoveKeyTask");
            await keyInstance.transform.DOMove(endPosition.position, duration).SetEase(Ease.InBack).ToUniTask();

            await KeyScale(keyInstance);

            // Debug.Log("SETCOINMOVECOINTASK!" + needToSum / coinAmount);

        }

        private async UniTask KeyScale(GameObject keyInstance)
        {
            if (keyReactionTween == null)
            {
                keyReactionTween = keyInstance.transform.DOScale(new Vector3(2f, 2f, 2f), duration).SetEase(Ease.InOutElastic);
                await keyReactionTween.ToUniTask();
                keyReactionTween = null;

               // await keyInstance.transform.DOPunchPosition(new Vector3(0, 30, 0), Random.Range(0, 1f)).SetEase(Ease.InOutElastic).ToUniTask();
            }

        }


    }
}