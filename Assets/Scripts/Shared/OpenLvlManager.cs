using TowerDefense;
using UnityEngine;

namespace QuizCinema
{
    public class OpenLvlManager : MonoBehaviour
    {
		[SerializeField] private MapLevel _currentMapLevel;

		private void Start()
		{
			ActivatePanel.OnMapLevel += OnMapLevel;
		}

		private void OnDestroy()
		{
			ActivatePanel.OnMapLevel -= OnMapLevel;
		}

		private void OnMapLevel(MapLevel mapLevel)
		{
			_currentMapLevel = mapLevel;
		}

		public void StartLevel()
		{
			_currentMapLevel.LoadLevel();
		}
	}
}