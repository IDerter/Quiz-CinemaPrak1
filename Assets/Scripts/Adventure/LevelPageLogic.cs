using QuizCinema;
using SpaceShooter;
using System.Collections;
using TowerDefense;
using UnityEngine;

public class LevelPageLogic : MonoBehaviour
{
    [SerializeField] private MapLevel[] LevelsByPage;
	[SerializeField] private ScrollRectSnap _scrollRectSnap;

	private void Start()
	{
		var index = MapCompletion.Instance.LastLevelAdventureIndex / LevelsByPage.Length;

		StartCoroutine(DelayStartSnap(index, LevelSequenceController.Instance.TimeAnimClick));
		Debug.Log($"LevelPageLogic index {index}");
	}

	private IEnumerator DelayStartSnap(int index, float time)
	{
		yield return new WaitForSeconds(time);

		_scrollRectSnap.SnapToItem(index);
	}
}
