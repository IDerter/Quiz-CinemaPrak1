using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceShooter;

namespace QuizCinema
{
    public class StorageEpisode : SingletonBase<StorageEpisode>
    {
        [SerializeField] private Episode[] _episodes;
        public Episode[] GetEpisodes { get { return _episodes; } set { _episodes = value; } }
    }
}