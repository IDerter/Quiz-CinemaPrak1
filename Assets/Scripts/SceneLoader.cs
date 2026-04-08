using System;
using System.Collections;
using TowerDefense;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YG; // Обязательно добавляем пространство имен плагина

namespace QuizCinema
{
    public class SceneLoader : MonoBehaviour
    {
        public static event Action UpdateDataEpisodes;

        [SerializeField] private Button _buttonContinue;
        [SerializeField] private GameObject _panelSettings;

        private const string _sceneLevelMap = "LevelMap";
        private const string _sceneMainMenu = "MainMenu";
        private const string _sceneShop = "Shop";
        [SerializeField] private string[] _lvls;
        [SerializeField] private string[] _stars;
        [SerializeField] private string[] _bars;

        [SerializeField] private AudioClip _audioClipButtonClick;

        private void Start()
        {
            if (_buttonContinue != null)
            {
                // Вместо проверки файлов на диске, мы проверяем реальный прогресс в данных игры.
                // Кнопка "Продолжить" активна, если игрок прошел хотя бы один уровень,
                // завершил обучение или имеет накопленные очки.
                bool hasProgress = MapCompletion.Instance.GetLastAdventureLevelIndex() > 0 ||
                                   MapCompletion.Instance.CompleteLearning ||
                                   MapCompletion.Instance.TotalScoreLvls > 500; // 500 - это ваш _signBonus

                _buttonContinue.interactable = hasProgress;
            }
        }

        public void NewGame()
        {
            // Удаляем старый FileHandler.Reset(...), так как мы больше не используем файлы.
            // Вместо этого вызываем методы сброса, которые мы уже написали в MapCompletion.

            BoostsManager.Instance.ResetBoostSave();
            BoostsManager.Instance.ResetListSaveBoosts();

            SkinManager.Instance.ResetSkinSave();

            MapCompletion.ResetEpisodeResult();
            MapCompletion.ResetLearningAndBarProgress();

            foreach (var lvl in _lvls)
            {
                PlayerPrefs.SetInt(lvl, 0);
                foreach (var star in _stars)
                    PlayerPrefs.SetInt(lvl + star, 0);
            }

            foreach (var bar in _bars)
            {
                PlayerPrefs.SetInt(bar, 0);
            }

            PlayerPrefs.Save(); // Сохраняем обнуленные PlayerPrefs
            YG2.SaveProgress(); // Отправляем сброшенный прогресс в облако Яндекса

            SceneManager.LoadScene(_sceneLevelMap);
        }

        IEnumerator WaitResetPath()
        {
            yield return new WaitForSeconds(1f);
            UpdateDataEpisodes?.Invoke();
            SceneManager.LoadScene(_sceneLevelMap);
        }

        public void OpenLevelMap()
        {
            AudioManager.Instance.PlaySoundAudioClip(_audioClipButtonClick);
            SceneManager.LoadScene(_sceneLevelMap);
        }

        public void OpenSceneShop()
        {
            AudioManager.Instance.PlaySoundAudioClip(_audioClipButtonClick);
            SceneManager.LoadScene(_sceneShop);
        }

        public void OpenMainMenu()
        {
            AudioManager.Instance.PlaySoundAudioClip(_audioClipButtonClick);
            SceneManager.LoadScene(_sceneMainMenu);
        }

        public void OpenSettings()
        {
            AudioManager.Instance.PlaySoundAudioClip(_audioClipButtonClick);
            _panelSettings.SetActive(true);
        }

        public void CloseSettings()
        {
            AudioManager.Instance.PlaySoundAudioClip(_audioClipButtonClick);
            _panelSettings.SetActive(false);
        }

        public void Quit()
        {
            AudioManager.Instance.PlaySoundAudioClip(_audioClipButtonClick);
            Application.Quit();
        }
    }
}