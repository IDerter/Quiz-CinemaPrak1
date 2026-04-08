using UnityEngine;
using UnityEngine.UI;

namespace QuizCinema
{
    public class AdaptUI : MonoBehaviour
    {
        [SerializeField] private CanvasScaler _uiScale;

        private float _referenceRatio;
        private int _currentMatch;

        private void Awake()
        {
            _referenceRatio = _uiScale.referenceResolution.x / _uiScale.referenceResolution.y;
            _currentMatch = (int)_uiScale.matchWidthOrHeight;
        }

        private void Update()
        {
            var currentRatio = (float)Screen.width / Screen.height;
            var potentialMatch = currentRatio < _referenceRatio ? 1 : 0;

            if (potentialMatch == _currentMatch)
                return;

            _currentMatch = potentialMatch;
            _uiScale.matchWidthOrHeight = _currentMatch;
        }
    }
}