using Cysharp.Threading.Tasks;
using SpaceShooter;
using System;
using System.Collections;
using TowerDefense;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YG;

namespace QuizCinema
{
	public class LevelMapButtonController : MonoBehaviour
    {
        public event Action OnDropInventory;

        private const string _sceneLevelMap = "LevelMap";
        private const string _sceneMainMenu = "MainMenu";
        private const string _sceneClassicRegime = "ClassicRegime";

        [SerializeField] private GameObject _errorMapLoading;

        [SerializeField] private ClassicRegime _classicRegime;

        private bool _isAdInProgress = false;
        [SerializeField] protected GameObject _retryButton;


        public void ReturnToMainMenu()
        {
            StartCoroutine(LoadSceneWithDelay(_sceneMainMenu));
        }

        private IEnumerator LoadSceneWithDelay(string sceneName)
        {
            yield return new WaitForSeconds(LevelSequenceController.Instance.TimeAnimClick);

            SceneManager.LoadScene(sceneName);
        }

        private IEnumerator LoadSceneWithDelay(string sceneName, float timeWait)
        {
            yield return new WaitForSeconds(timeWait);

            SceneManager.LoadScene(sceneName);
        }

        public void LoadLevelMap()
        {
            StartCoroutine(LoadSceneWithDelay(_sceneLevelMap));
        }

        public async void LoadClassicRegime()
        {
            Debug.Log(!BackgroundDownloader.Instance.IsMapDataLoaded + " " + MapCompletion.Instance.LastClassicIndex);
            if (!BackgroundDownloader.Instance.IsMapDataLoaded && !(MapCompletion.Instance.LastClassicIndex >= 0 && MapCompletion.Instance.LastClassicIndex < 5))
			{
                _errorMapLoading.SetActive(true);
                return;
			}

            var needMoney = (MapCompletion.Instance.TotalScoreLvls + MapCompletion.Instance.TotalAdsMoney - MapCompletion.Instance.MoneyShop - MapCompletion.Instance.SkinShop) - _classicRegime.CostAmount;
            Debug.Log(needMoney + " needMoney");

            if (needMoney < 0)
            {
                _classicRegime.ShowClassicPanel(Math.Abs(needMoney));
                return;
            }
            _classicRegime.BuyClassicRegime();
            MapCompletion.Instance.CountLoadClassicRegime++;
            if (MapCompletion.Instance.CountLoadClassicRegime % 3 == 0 && YG2.platform != "YandexGames")
			{

                _isAdInProgress = true;
                _retryButton.GetComponent<Button>().interactable = false; // Блокируем кнопку

                var adShown = new UniTaskCompletionSource();
                Action onAdClosed = () => adShown.TrySetResult();
                InterstitialAds.OnInterstitialAdClosed += onAdClosed;

                AdsManager.Instance._interstitialAds.ShowInterstitialAd();

                await adShown.Task; // Ожидаем завершения рекламы
                InterstitialAds.OnInterstitialAdClosed -= onAdClosed;

                _isAdInProgress = false;
                _retryButton.GetComponent<Button>().interactable = true; // Разблокируем кнопку
                
            }
            else if (YG2.platform == "YandexGames")
			{
                _isAdInProgress = true;
                _retryButton.GetComponent<Button>().interactable = false; // Блокируем кнопку

                var adShown = new UniTaskCompletionSource();
                Action onAdClosed = () => adShown.TrySetResult();
                InterstitialAds.OnInterstitialAdClosed += onAdClosed;

                AdsManager.Instance._interstitialAds.ShowInterstitialAd();

                await adShown.Task; // Ожидаем завершения рекламы
                InterstitialAds.OnInterstitialAdClosed -= onAdClosed;

                _isAdInProgress = false;
                _retryButton.GetComponent<Button>().interactable = true; // Разблокируем кнопку
            }

            StartCoroutine(LoadSceneWithDelay(_sceneClassicRegime, 1.5f));
        }


        public void LoadSceneByName(string scene)
        {
            StartCoroutine(LoadSceneWithDelay(scene));
        }

        public void LoadSceneBar()
        {
            LevelSequenceController.Instance.ExitToBar();
        }

        IEnumerator StartScene(string scene)
        {
            yield return new WaitForSeconds(0.5f);
            SceneManager.LoadScene(scene);
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}