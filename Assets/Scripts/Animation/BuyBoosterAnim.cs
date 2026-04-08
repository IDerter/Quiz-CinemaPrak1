using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace QuizCinema
{
	public class BuyBoosterAnim : MonoBehaviour
    {
        [SerializeField] private GameObject boosterPrefab;

        [SerializeField] private Transform boosterParent;

        [SerializeField] private Transform spawnLocation;

        [SerializeField] private Transform endPosition;

        [SerializeField] private float duration = 2;

        [SerializeField] private float minX;

        [SerializeField] private float maxX;

        [SerializeField] private float minY;

        [SerializeField] private float maxY;

        private GameObject booster;


		[Button()]
        public async void BoosterAnimStart()
        {
            Debug.Log("StartKeyAnim");
            GameObject keyInstance = Instantiate(boosterPrefab, boosterParent);
            float xPosition = spawnLocation.position.x + Random.Range(minX, maxX);
            float yPosition = spawnLocation.position.y + Random.Range(minY, maxY);

            keyInstance.transform.position = new Vector3(xPosition, yPosition);
            // await keyInstance.transform.DOPunchPosition(new Vector3(0, 30, 0), Random.Range(0, 1f)).SetEase(Ease.InOutElastic)
            //    .ToUniTask();
            booster = keyInstance;

 
            //await booster.transform.GetComponent<Image>().DOFade(1, duration / 2);
            await UniTask.Delay(TimeSpan.FromSeconds(0.01f));

            MoveStarTask();
            
        }


        private void MoveStarTask()
        {
            UniTask moveKeyTask = new UniTask();
            moveKeyTask = MoveStarTask(booster);

        }

        private async UniTask MoveStarTask(GameObject keyInstance)
        {
            await keyInstance.transform.DOMove(endPosition.position, duration * 2).SetEase(Ease.InOutBack).ToUniTask();
            Destroy(keyInstance);
        }
    }
}