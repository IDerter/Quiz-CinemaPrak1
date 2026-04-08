using QuizCinema;
using System;
using TowerDefense;
using UnityEngine;

public class OpenNewLevels : MonoBehaviour
{
    [SerializeField] private int _lengthLevels = 5;

    [SerializeField] private string[] _barNames;

    private void Start()
    {
        var lastIndex = MapCompletion.Instance.LastLevelAdventureIndex;
        var barIndex = lastIndex / _lengthLevels;

        // ѕровер€ем, не обрабатывали ли мы уже этот бар
        var alreadyProcessed = PlayerPrefs.GetInt($"Bar_{barIndex}_Processed", 0) == 1;

        Debug.Log($"OPEN NEW LEVELS открыт ли бар с индексом {barIndex} - {MapCompletion.Instance.GetOpensBar[barIndex]} в обработке ли: { alreadyProcessed}" );
        if (!MapCompletion.Instance.GetOpensBar[barIndex] && !alreadyProcessed)
        {
            // Ћогика дл€ прогрессивной загрузки контента
            if (barIndex > 0)
            {
                int firstLevelToLoad = (barIndex) * 5 + 2;
                int lastLevelToLoad = (barIndex + 1) * 5 + 1;

                Debug.Log($"[BarAnim] Bar {barIndex} opened. Preloading levels from {firstLevelToLoad} to {lastLevelToLoad}.");
                BackgroundDownloader.Instance.EnqueueLevelRange(firstLevelToLoad, lastLevelToLoad, true);
            }

            MapCompletion.Instance.GetOpensBar[barIndex] = true;
            MapCompletion.SaveBarProgress();

            // ќтмечаем, что этот бар уже обработан
            PlayerPrefs.SetInt($"Bar_{barIndex}_Processed", 1);
            PlayerPrefs.Save();
        }
    }
}
