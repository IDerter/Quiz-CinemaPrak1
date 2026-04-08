using UnityEngine;
using System;

namespace QuizCinema
{
    public enum TypeAudioMixer
    {
        Music,
        SFX
    }
    [Serializable]
    public class Sound
    {
        
        [SerializeField] private TypeAudioMixer _typeMixer;
        public TypeAudioMixer TypeMixer => _typeMixer;
        
        [SerializeField] private string _name;
        public string Name => _name;

        [SerializeField] private AudioClip _clip;
        public AudioClip Clip => _clip;

        [SerializeField] private SoundParameters _parameters;
        public SoundParameters Parameters => _parameters;

        [SerializeField] private AudioSource _source;
        public AudioSource Source { get { return _source; } set { _source = value; } }

        public void Play()
        {
            if (_source != null)
            {
                _source.clip = _clip;

                _source.loop = _parameters.Loop;
                _source.pitch = _parameters.Pitch;
                _source.volume = _parameters.Volume;

                _source.Play();
            }
        }

        public void Stop()
        {
            _source.Stop();
        }
    }
}