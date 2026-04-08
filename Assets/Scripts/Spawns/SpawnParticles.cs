using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuizCinema
{
    public class SpawnParticles : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _spawnObject;
        [SerializeField] private Transform _spawnParent;


        public void Spawn()
        {
            var scale = _spawnObject.gameObject.transform.localScale;
            var obj = Instantiate(_spawnObject, _spawnParent);
            obj.transform.SetParent(null);
            obj.transform.localScale = scale;
        }
    }
}