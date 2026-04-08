using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShooter
{
    [CreateAssetMenu]
    public class Episode : ScriptableObject
    {
        [SerializeField] private string _episodeName;
        public string EpisodeName => _episodeName;

        [SerializeField] private string[] _levels;

        public string[] Levels => _levels;

        [SerializeField] private Sprite _previewImage;

        public Sprite PreviewImage => _previewImage;

        [SerializeField] private int _episodeID;
        public int EpisodeID => _episodeID;
    }
}

