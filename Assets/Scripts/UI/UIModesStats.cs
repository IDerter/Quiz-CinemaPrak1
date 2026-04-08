using TMPro;
using TowerDefense;
using UnityEngine;

namespace QuizCinema
{
	public class UIModesStats : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _textAdventureLastLevel;
		[SerializeField] private TextMeshProUGUI _textAdventureMaxLevelClassic;
		[SerializeField] private TextMeshProUGUI _textMaxClassicRecord;

		private void Awake()
		{
			MapCompletion.OnModesLevelStatsUpdate += OnModesLevelStatsUpdate;
		}

		private void Start()
		{
			_textAdventureLastLevel.text = MapCompletion.Instance.LastLevelAdventureIndex.ToString();
			_textAdventureMaxLevelClassic.text = MapCompletion.Instance.MaxLevelsAdventure.ToString();
			_textMaxClassicRecord.text = MapCompletion.Instance.MaxRecordClassic.ToString();

			Debug.Log(MapCompletion.Instance.LastLevelAdventureIndex.ToString());
		}

		private void OnDestroy()
		{
			MapCompletion.OnModesLevelStatsUpdate -= OnModesLevelStatsUpdate;
		}

		public void OnModesLevelStatsUpdate()
		{
			var lastLevelIndex = MapCompletion.Instance.GetLastAdventureLevelIndex();

			_textAdventureLastLevel.text = lastLevelIndex.ToString();
			_textMaxClassicRecord.text = MapCompletion.Instance.MaxRecordClassic.ToString();
		}
	}
}