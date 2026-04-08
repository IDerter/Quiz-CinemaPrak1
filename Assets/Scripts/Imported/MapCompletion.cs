using UnityEngine.SceneManagement;
using UnityEngine;
using SpaceShooter; // Предполагаю, что это ваши неймспейсы
using System;
using QuizCinema;
using UnityEngine.Events;
using YG;

namespace TowerDefense
{
    public class MapCompletion : SingletonBase<MapCompletion>
    {
        public static event Action OnScoreUpdate;
        public static event Action OnBarOpenInfoUpdate;
        public static event Action OnLearningSave;
        public static event Action OnModesLevelStatsUpdate;

        // Делаем класс публичным, чтобы плагин YG мог его сериализовать в SavesYG
        [Serializable]
        public class EpisodeScore
        {
            [SerializeField] private int _episodeID;
            [SerializeField] private int _stars;
            [SerializeField] private int _scoreLvl;
            [SerializeField] private string _lvlName;
            [SerializeField] private int _maxScoreLvl;

            public int EpisodeID { get => _episodeID; set => _episodeID = value; }
            public int Stars { get => _stars; set => _stars = value; }
            public int ScoreLvl { get => _scoreLvl; set => _scoreLvl = value; }
            public string LvlName { get => _lvlName; set => _lvlName = value; }
            public int MaxScoreLvl { get => _maxScoreLvl; set => _maxScoreLvl = value; }
        }

        // Этот массив настраивается в инспекторе (базовые уровни)
        [SerializeField] private EpisodeScore[] _completionDataUnity;

        private Episode _currentEpisode;

        [SerializeField] private int _totalStars;
        public int TotalStars => _totalStars;

        [SerializeField] private int _totalScoreLvls;
        public int TotalScoreLvls
        {
            get => _totalScoreLvls;
            set => _totalScoreLvls = Mathf.Clamp(value, 0, int.MaxValue);
        }

        [SerializeField] private int _moneyShop;
        public int MoneyShop { get => _moneyShop; set => _moneyShop = value; }

        [SerializeField] private int _skinShop;
        public int SkinShop { get => _skinShop; set => _skinShop = value; }

        public int MoneySpentClassic
        {
            get => YG2.saves.moneySpentClassic;
            set => YG2.saves.moneySpentClassic = value;
        }

        [SerializeField] private int _lastLevelAdventureIndex = 0;
        public int LastLevelAdventureIndex => _lastLevelAdventureIndex;
        public int MaxLevelsAdventure => _completionDataUnity.Length;

        [SerializeField] public int CountLoadClassicRegime = 0;
        private int _signBonus = 500;

        // --- Свойства, которые теперь работают напрямую с облаком (YG2.saves) ---

        public int TotalAdsMoney
        {
            get => YG2.saves.totalAdsMoney;
            set => YG2.saves.totalAdsMoney = Mathf.Clamp(value, 0, int.MaxValue);
        }

        public int CountLvlFinished { get => YG2.saves.countLvlFinished; set => YG2.saves.countLvlFinished = value; }
        public bool[] GetOpensBar { get => YG2.saves.isOpenBar; set => YG2.saves.isOpenBar = value; }
        public bool CompleteLearning  { get => YG2.saves.completeLearning; set => YG2.saves.completeLearning = value; }
        public bool[] LearnSteps { get => YG2.saves.learnSteps; set => YG2.saves.learnSteps = value; }
        public int LastClassicIndex { get => YG2.saves.lastClassicIndex; set => YG2.saves.lastClassicIndex = value; }
        public int MaxRecordClassic { get => YG2.saves.maxRecordClassic; set => YG2.saves.maxRecordClassic = value; }

        private new void Awake()
        {
            base.Awake();
        }

        private void OnEnable()
        {
            // Подписываемся на событие загрузки данных с серверов Яндекса
            YG2.onGetSDKData += LoadData;
        }

        private void OnDisable()
        {
            YG2.onGetSDKData -= LoadData;
        }

        private void Start()
        {
            // Если плагин уже инициализирован (например, при перезагрузке сцены), загружаем сразу
            if (YG2.isSDKEnabled)
            {
                LoadData();
            }

            _moneyShop = BoostsManager.Instance.GetMoneyForBoosts;
            _skinShop = SkinManager.Instance.GetMoneyForSkins;

            SkinManager.Instance.OnBuySkin += OnBuy;
            BoostsManager.Instance.OnBuyBoost += OnBuy;
        }

        private void OnDestroy()
        {
            if (SkinManager.Instance != null) SkinManager.Instance.OnBuySkin -= OnBuy;
            if (BoostsManager.Instance != null) BoostsManager.Instance.OnBuyBoost -= OnBuy;
        }

        private void OnBuy()
        {
            OnScoreUpdate?.Invoke();
        }

        // Метод загрузки данных из плагина в вашу логику
        public virtual void LoadData()
        {
            _totalStars = 0;
            _totalScoreLvls = 0;

            // Проверяем, есть ли сохранения уровней в облаке
            if (YG2.saves.completionData != null && YG2.saves.completionData.Length > 0)
            {
                // Переносим сохраненные данные в рабочий массив Unity
                for (int i = 0; i < _completionDataUnity.Length && i < YG2.saves.completionData.Length; i++)
                {
                    _completionDataUnity[i] = YG2.saves.completionData[i];
                    var episodeScore = _completionDataUnity[i];

                    _totalStars += episodeScore.Stars;
                    _totalScoreLvls += episodeScore.ScoreLvl;

                    if (episodeScore.Stars > 0)
                    {
                        _lastLevelAdventureIndex = i;
                    }
                }
                _totalScoreLvls += _signBonus;
            }
            else
            {
                ResetEpisodeResult();
                _totalScoreLvls += _signBonus;
            }

            // Инициализация баров, если это первый запуск
            if (YG2.saves.isOpenBar == null || YG2.saves.isOpenBar.Length == 0)
            {
                YG2.saves.isOpenBar = new bool[StorageEpisode.Instance.GetEpisodes.Length];
                YG2.saves.isOpenBar[0] = true;
                YG2.SaveProgress(); // Сохраняем начальное состояние
            }

            // Инициализация обучения
            if (YG2.saves.learnSteps == null || YG2.saves.learnSteps.Length == 0)
            {
                YG2.saves.learnSteps = new bool[10];
                YG2.SaveProgress();
            }

            // Проверка на завершение обучения
            if (YG2.saves.learnSteps.Length > 4 && YG2.saves.learnSteps[4] && !YG2.saves.completeLearning)
            {
                SaveFinishLearining();
            }

            _moneyShop = BoostsManager.Instance.GetMoneyForBoosts;
            _skinShop = SkinManager.Instance.GetMoneyForSkins;

            OnModesLevelStatsUpdate?.Invoke();
            OnScoreUpdate?.Invoke();
        }

        public int GetLastAdventureLevelIndex()
        {
            for (int i = 0; i < _completionDataUnity.Length; i++)
            {
                if (_completionDataUnity[i].Stars > 0)
                {
                    _lastLevelAdventureIndex = i + 1;
                }
            }
            return _lastLevelAdventureIndex;
        }

        // --- Методы сохранения (теперь используют YG2.SaveProgress) ---

        public static void SaveLearningProgress()
        {
            OnLearningSave?.Invoke();
            YG2.SaveProgress();
        }

        public static void SaveFinishLearining()
        {
            Instance.CompleteLearning = true;
            YG2.SaveProgress();
        }

        public static void SaveLvlFinished()
        {
            // Значение уже обновлено через свойство, просто отправляем в облако
            YG2.SaveProgress();
        }

        public static void SaveMaxRecordClassic()
        {
            YG2.SaveProgress();
        }

        public static void SaveLastIndexClassic()
        {
            YG2.SaveProgress();
        }

        public static void SaveAds()
        {
            YG2.SaveProgress();
        }

        public static void ResetLearningAndBarProgress()
        {
            YG2.saves.learnSteps = new bool[10];
            YG2.saves.isOpenBar = new bool[YG2.saves.isOpenBar.Length];
            YG2.saves.isOpenBar[0] = true;
            YG2.saves.completeLearning = false;

            YG2.SaveProgress();
        }

        public static void SaveBarProgress()
        {
            OnBarOpenInfoUpdate?.Invoke();
            YG2.SaveProgress();
        }

        public static void SaveEpisodeResult(int levelStars, int levelScore)
        {
            if (Instance == null) return;

            string currentSceneName = SceneManager.GetActiveScene().name;
            int i = 0;

            for (int j = 0; j < Instance._completionDataUnity.Length; j++)
            {
                var item = Instance._completionDataUnity[j];
                Episode episode = StorageEpisode.Instance.GetEpisodes[item.EpisodeID - 1];

                if (i >= 5) i = 0;

                if (episode == LevelSequenceController.Instance.CurrentEpisode && episode.Levels[i] == currentSceneName)
                {
                    if (episode.Levels.Length > i)
                        item.LvlName = currentSceneName;

                    SaveStarsAndScoreLvls(item, levelStars, levelScore);

                    var score = item.ScoreLvl;
                    item.MaxScoreLvl = score > item.MaxScoreLvl ? score : item.MaxScoreLvl;

                    if (item.Stars > 0)
                    {
                        Instance._lastLevelAdventureIndex = j + 1;
                    }
                }
                i++;
            }

            SaveAds(); // SaveAds вызовет YG2.SaveProgress()
            OnScoreUpdate?.Invoke();
        }

        private static void SaveStarsAndScoreLvls(EpisodeScore item, int levelStars, int levelScore)
        {
            if (item.ScoreLvl < levelScore)
            {
                Instance._totalScoreLvls += levelScore - item.ScoreLvl;
                item.ScoreLvl = levelScore;

                Instance._totalStars += levelStars - item.Stars;
                item.Stars = levelStars;

                // Обновляем данные в объекте сохранений плагина
                YG2.saves.completionData = Instance._completionDataUnity;
                YG2.SaveProgress();

                Debug.Log("Произошел сейв " + levelScore);
            }

            OnScoreUpdate?.Invoke();
        }

        public static void ResetEpisodeResult()
        {
            if (Instance == null) return;

            foreach (var item in Instance._completionDataUnity)
            {
                if (item.Stars > 0 || item.ScoreLvl > 0)
                {
                    item.Stars = 0;
                    item.ScoreLvl = 0;
                    item.MaxScoreLvl = 0;
                }
            }

            Instance._totalStars = 0;
            Instance._totalScoreLvls = 0;
            Instance._moneyShop = 0;
            Instance._skinShop = 0;

            Instance.TotalAdsMoney = 0; // Сброс рекламы
            Instance.CompleteLearning = false;

            // Записываем чистый массив в облако
            YG2.saves.completionData = Instance._completionDataUnity;
            YG2.SaveProgress();

            OnScoreUpdate?.Invoke();
        }

        // --- Вспомогательные методы (Остаются без изменений) ---

        public int GetLvlStars(string episode)
        {
            foreach (var data in _completionDataUnity)
            {
                if (data.LvlName == episode) return data.Stars;
            }
            return 0;
        }

        public int GetLvlScore(string episodeName)
        {
            foreach (var data in _completionDataUnity)
            {
                if (data.LvlName == episodeName) return data.ScoreLvl;
            }
            return 0;
        }

        public int GetEpisodeStars(int idEpisode)
        {
            var starsEpisode = 0;
            foreach (var data in _completionDataUnity)
            {
                if (data.EpisodeID == idEpisode) starsEpisode += data.Stars;
            }
            return starsEpisode;
        }

        public int GetLvlNumber(string episodeName)
        {
            int i = 0;
            foreach (var data in _completionDataUnity)
            {
                if (data.LvlName == episodeName) return i;
                i++;
            }
            return 0;
        }

        public int GetLvlMaxScore(string episodeName)
        {
            foreach (var data in _completionDataUnity)
            {
                if (data.LvlName == episodeName) return data.MaxScoreLvl;
            }
            return 0;
        }

        public int GetMaxCountClassicRegime()
        {
            return MaxRecordClassic;
        }

        public int GetSumLvlScore(int episodeID)
        {
            int sum = 0;
            foreach (var data in _completionDataUnity)
            {
                if (data.EpisodeID == episodeID) sum += data.ScoreLvl;
            }
            return sum;
        }
    }
}