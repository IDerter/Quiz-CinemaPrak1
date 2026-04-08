using UnityEngine;
using SpaceShooter;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using QuizCinema;
using System.Collections;
using Spine.Unity;

namespace TowerDefense
{
    public class MapLevel : MonoBehaviour
    {
        [SerializeField] private Episode _episode;
        public Episode Episode { get { return _episode; } set { _episode = value; } }
        [SerializeField] private GameObject _resultPanel;
        public GameObject ResultPanel { get { return _resultPanel; } set { _resultPanel = value; } }
        [SerializeField] private Image[] _resultImages;
        public Image[] ResultImages { get { return _resultImages; } set { _resultImages = value; } }
        [SerializeField] private Sprite[] _spritesStarsYellow;
        public Sprite[] SpritesStarts { get { return _spritesStarsYellow; } set { _spritesStarsYellow = value; } }

        public bool IsComplete => gameObject.activeSelf && _resultPanel.activeSelf;

        [SerializeField] private TypeStarts _type;
        public TypeStarts GetType => _type;
        [SerializeField] private Image _lockImage;
        public Image GetLockImage => _lockImage;
        [SerializeField] private SkeletonGraphic _lockAnim;
        public SkeletonGraphic LockAnim { get { return _lockAnim; } set { _lockAnim = value; } }

        [SerializeField] private Image _overlayImage;
        public Image OverlayImage => _overlayImage;
        [SerializeField] private BarAnim _barAnim;
        public BarAnim BarAnim => _barAnim;
        [SerializeField] private bool _locked = true;
        public bool Lock { get { return _locked; } set { _locked = value; } }
        [SerializeField] private GameObject _buttonLoadScene;
        public GameObject ButtonLoadScene => _buttonLoadScene;


        public virtual void LoadLevel()
        {
            Debug.Log("LoadLevel" + gameObject.name);

            StartCoroutine(LoadSceneWithDelay());
        }

        private IEnumerator LoadSceneWithDelay()
        {
            yield return new WaitForSeconds(LevelSequenceController.Instance.TimeAnimClick);
            Debug.Log("LoadLevel" + MapCompletion.Instance.GetLvlNumber(gameObject.name));
            LevelSequenceController.Instance.StartEpisode(_episode, MapCompletion.Instance.GetLvlNumber(gameObject.name), gameObject.name);
        }

        public virtual int Initialize()
        {
            var indexEpisode = _episode.EpisodeID;
            var starsEpisode = MapCompletion.Instance.GetEpisodeStars(indexEpisode);

            var sumLvlScore = MapCompletion.Instance.GetSumLvlScore(indexEpisode);
            //var needSumToOpenBar = StorageBarsInfo.Instance.InfoBars[indexEpisode - 1].NeedSumScore;
            //int score = Convert.ToInt32(Math.Round((double)(starsEpisode / _episode.Levels.Length)));
            var needStarsToOpenBar = StorageBarsInfo.Instance.InfoBars[indexEpisode - 1].NeedStarsScore;
            var scoreBar = starsEpisode - needStarsToOpenBar;

            Debug.Log("StarsEpisode: " + starsEpisode + " Need stars to open bar: " + needStarsToOpenBar);

            var starUpBar = 0;
            if (scoreBar == 0)
                starUpBar = 1;
            else if (scoreBar > 0 && scoreBar <= 3)
                starUpBar = 2;
            else if (starsEpisode <= 15)
                starUpBar = 3;

            var checkBarOpen = starsEpisode >= needStarsToOpenBar ? starUpBar : 0;

            //var check = sumLvlScore > needSumToOpenBar ? score : 0;
            Debug.Log(checkBarOpen + " Can we open the bar! " + gameObject.name + $"SumLvlScore : {sumLvlScore} needStarsToOpenBar: {needStarsToOpenBar} " + indexEpisode);
            Debug.Log(starsEpisode + "Number of stars!" + " ScoreBar: " + scoreBar);

           // if(checkBarOpen > 0)
             //   MapCompletion.Instance.GetOpensBar[indexEpisode] = true;

            int stars = MapCompletion.Instance.GetLvlStars(gameObject.name);
            Debug.Log($"MapLevel '{gameObject.name}': Got {stars} stars from MapCompletion.");


            if (_type == TypeStarts.Lvl)
            {
                Debug.Log(starsEpisode + "Number of stars per table");
                return ShowStarsResult(stars);
            }

            else if (_type == TypeStarts.Bar)
            {
                return ShowStarsResult(checkBarOpen);
            }

            return 0;
        }

        private int ShowStarsResult(int value)
        {
            _resultPanel?.SetActive(value >= 0);

            if (ResultImages.Length >= value)
            {
                for (int i = 0; i < value; i++)
                {
                    _resultImages[i].sprite = _spritesStarsYellow[i];
                    if (_resultImages[i].TryGetComponent(out StarsTableAnim starsTableAnim))
                    {
                        if (_resultImages[i].TryGetComponent(out FadeImage fadeImage))
                        {
                            fadeImage.FadeInStartAnim();
                        }
                        StartCoroutine(AnimStartDelay(starsTableAnim));
                    }
                }
                if (value >= 0)
				{
                    for (int i = value; i < 3; i++)
					{
                        if (_resultImages[i].TryGetComponent(out FadeImage fadeImage))
                        {
                            fadeImage.FadeInStartAnim();
                        }
                    }
				}
            }
            else
            {
                Debug.LogWarning("Error with stars. To much value");
                Debug.Log(value);
            }

            return value;
        }

        private IEnumerator AnimStartDelay(StarsTableAnim starsTableAnim)
		{
            yield return new WaitForSeconds(LevelSequenceController.Instance.TimeAnimClick);

            starsTableAnim.StarAnimStart();
        }
    }
}