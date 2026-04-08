using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SpaceShooter
{
    public interface ILevelCondition
    {
        bool IsCompleted { get; }
    }

    public class LevelController : SingletonBase<LevelController>
    {
        [SerializeField] protected float _referenceTime;
        public float ReferenceTime => _referenceTime;

        [SerializeField] protected UnityEvent _eventLevelCompleted;

        private ILevelCondition[] _conditions; 

        [SerializeField] private float _defaultTimeToWin = 60;
        public float StandartTimeToWin => _defaultTimeToWin;

        private bool _isLevelCompleted;

        private float _levelTime;
        public float LevelTime => _levelTime;


        protected void Start()
        {
            _conditions = GetComponentsInChildren<ILevelCondition>();
        }

        private void Update()
        {
            if (!_isLevelCompleted)
            {
                _levelTime += Time.deltaTime;

                CheckLevelConditions();
            }
        }

        private void CheckLevelConditions()
        {
            if (_conditions == null || _conditions.Length == 0) return;

            int numCompleted = 0;

            foreach (var v in _conditions)
            {
                if (v.IsCompleted)
                {
                    numCompleted++;
                    Debug.Log("CompletedConditions");
                }
            }

            if(numCompleted == _conditions.Length)
            {
                _isLevelCompleted = true;
                _eventLevelCompleted?.Invoke();

                LevelSequenceController.Instance?.FinishCurrentLevel(true);
            }
        }
    }
}

