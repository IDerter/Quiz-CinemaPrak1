using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuizCinema
{
    public class SpawnContent : MonoBehaviour
    {
        [SerializeField] private GameObject[] _questionsContentArea;
        

        private void Awake()
        {
            for (int i = 0; i < _questionsContentArea.Length; i++)
            {
                Instantiate(_questionsContentArea[i], gameObject.transform);
            }
            Debug.Log("EndSpawn");
        }
    }
}