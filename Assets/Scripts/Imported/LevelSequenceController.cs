using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpaceShooter
{
    public class LevelSequenceController : SingletonBase<LevelSequenceController>
    {
        public static string _mapMenuSceneNickname = "LevelMap";
        public static string _mainMenuSceneNickname = "MainMenu";

        [SerializeField] private Episode _episodeForTest;
        public Episode CurrentEpisode { get; set; }

        public int CurrentLevel { get; private set; }

        public bool LastLevelResult { get; private set; }

        public PlayerStatistics LevelStatisics { get; private set; }

        private float _multiplierScore;

        [SerializeField] private int _maxScore = 0;
        public int MaxScore => _maxScore;

        [SerializeField] private int _allKills = 0;

        [SerializeField] private int _bestTime = int.MaxValue;
        [SerializeField] private float _timeAnimClick = 0.3f;
        public float TimeAnimClick => _timeAnimClick;

        private void Start()
        {
            _allKills = PlayerPrefs.GetInt("AllKills");
            _bestTime = PlayerPrefs.GetInt("BestTime");
            _maxScore = PlayerPrefs.GetInt("MaxScore");
        }

        public void StartEpisode(Episode episode, int currentLevelIndex, string episodeName)
        {
            CurrentEpisode = episode;
            // CurrentLevel = 0;
            CurrentLevel = currentLevelIndex;

            LevelStatisics = new PlayerStatistics();
            LevelStatisics.Reset();
            Debug.Log(CurrentEpisode + " " + CurrentLevel);
            SceneManager.LoadScene(episodeName);
        }

        public void RestartLevel()
        {
            //SceneManager.LoadScene(CurrentEpisode.Levels[CurrentLevel]);
            SceneManager.LoadScene(1);
        }

        public void LoadLevel(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }

        public void NextLevel()
        {
            //LevelStatisics.Reset();
            Debug.Log($"NextLevel {CurrentLevel}"); // ãëÿíóòü âî âòîðîì áàðå 1 óðîâåíü - èíäåêñ 6 èëè 5?
            CurrentLevel++;
            AnalyticsManager.Instance.NextLevelStats(CurrentLevel);
            Debug.Log(CurrentEpisode.Levels.Length + " " + CurrentLevel);
            //CalculateLevelStatistics();
            var correctIndex = CurrentLevel - (int.Parse(CurrentEpisode.EpisodeName.Substring(CurrentEpisode.EpisodeName.Length - 1)) - 1) * CurrentEpisode.Levels.Length;
            Debug.Log(correctIndex + " " + int.Parse(CurrentEpisode.EpisodeName.Substring(CurrentEpisode.EpisodeName.Length - 1)));

            if (CurrentEpisode.Levels.Length * int.Parse(CurrentEpisode.EpisodeName.Substring(CurrentEpisode.EpisodeName.Length - 1))  <= CurrentLevel) // ÍÀÄÎ ÐÅØÈÒÜ ÂÎÏÐÎÑ ÓÌÍÎÆÀÒÜ ÍÓÆÍÎ ÍÀ 3
            {
                Debug.Log(CurrentEpisode.Levels.Length * int.Parse(CurrentEpisode.EpisodeName.Substring(CurrentEpisode.EpisodeName.Length - 1)));
                SceneManager.LoadScene(_mapMenuSceneNickname);
            }
            else
            {
                Debug.Log(correctIndex + " " + CurrentEpisode.Levels[correctIndex].ToString());
                if (CurrentEpisode.Levels[correctIndex].ToString() == "Lvl8_2")
                {
                    SceneManager.LoadScene(_mapMenuSceneNickname);
                }
                else
                {
                    SceneManager.LoadScene(CurrentEpisode.Levels[correctIndex]);
                }
            }
        }

        public void ExitToBar()
        {
            if (CurrentEpisode != null)
            {
                SceneManager.LoadScene(CurrentEpisode.EpisodeName);
            }
        }

        public void ExitToClassicLevelMenu()
        {
            if (CurrentEpisode != null)
            {
                SceneManager.LoadScene(_mapMenuSceneNickname);
            }
        }

        public void FinishCurrentLevel(bool success)
        {
            Debug.Log("FINISH");

            LastLevelResult = success;

            //CalculateLevelStatistics();

            //ResultPanelController.Instance.ShowResults(success);
        }

    }

}
