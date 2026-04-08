using System;
using TowerDefense; // Подключаем пространство имен вашей игры

namespace YG
{
    public partial class SavesYG
    {
        // Массивы для прогресса
        public MapCompletion.EpisodeScore[] completionData = new MapCompletion.EpisodeScore[0];
        public bool[] isOpenBar = new bool[0];
        public bool[] learnSteps = new bool[10];

        // Одиночные переменные
        public int totalAdsMoney = 0;
        public int countLvlFinished = 0;
        public int maxRecordClassic = 0;
        public int lastClassicIndex = 0;
        public bool completeLearning = false;
        public int moneySpentClassic = 0;

        public string lastDailyBonusDate = "";
    }
}