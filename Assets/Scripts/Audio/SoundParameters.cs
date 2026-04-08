using UnityEngine;
using System;

namespace QuizCinema
{
    [Serializable]
    public struct SoundParameters
    {
        [Range(0,1)]
        [SerializeField] private float _volume;
        public float Volume => _volume;

        [Range(-3, 3)]
        [SerializeField] private float _pitch;
        public float Pitch => _pitch;

        [SerializeField] private bool _loop;
        public bool Loop => _loop;
    }
}