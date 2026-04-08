using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace QuizCinema
{
    public class DoTweenTest : MonoBehaviour
    {
        private void Start()
        {
            transform.DOMove(new Vector3(0.5f, -0.5f, 0.5f), 2);
        }
    }
}