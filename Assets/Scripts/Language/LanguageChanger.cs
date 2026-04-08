using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;

namespace QuizCinema
{
    public class LanguageChanger : SingletonBase<LanguageChanger>
    {
        public event Action OnChangeLanguage;
        [SerializeField] private Language[] _buttonsLanguage;

        [SerializeField] private int _index = 0;
        public int GetLanguageIndex => _index;

        private const string _clickSFX = "ClickSFX";
        private static bool _isSessionInitialized = false;
        private bool _isWaitingForSDK = false;

        private void Start()
        {
            // Начинаем процесс инициализации языка
            StartCoroutine(InitializeLanguageCoroutine());
        }

        private IEnumerator InitializeLanguageCoroutine()
        {
            Debug.Log("[LanguageChanger] Waiting for Yandex SDK initialization...");

            float timeout = 10f;
            float elapsed = 0f;

            // Ждем инициализацию Яндекс SDK (проверяем через доступные свойства)
            while (!IsYandexReady() && elapsed < timeout)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (IsYandexReady())
            {
                Debug.Log($"[LanguageChanger] Yandex SDK initialized. Platform: {YG2.platform}, Lang: {YG2.lang}");
            }
            else
            {
                Debug.LogWarning("[LanguageChanger] Yandex SDK initialization timeout");
            }

            // Теперь можно работать с YG2
            InitializeLanguage();
        }

        private bool IsYandexReady()
        {
            try
            {
                // Проверяем через доступные свойства YG2
                if (!string.IsNullOrEmpty(YG2.platform))
                {
                    // Если платформа определена, значит SDK инициализирован
                    return true;
                }

                // Альтернативная проверка через язык
                if (!string.IsNullOrEmpty(YG2.lang))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        private void InitializeLanguage()
        {
            int newIndex = _index;

            // Проверяем, что YG2 доступен
            try
            {
                if (YG2.platform == "YandexGames")
                {
                    var yandexLanguage = YG2.lang;
                    Debug.Log($"[LanguageChanger] Yandex language from URL: '{yandexLanguage}'");

                    // Определяем индекс по языку
                    int yandexIndex = 0; // По умолчанию английский

                    if (!string.IsNullOrEmpty(yandexLanguage))
                    {
                        if (yandexLanguage == "ru")
                            yandexIndex = 1;
                        else if (yandexLanguage == "be" || yandexLanguage == "kk" || yandexLanguage == "uk" || yandexLanguage == "uz")
                            yandexIndex = 1; // Для русскоязычных стран тоже ставим русский
                    }

                    // Проверяем, первый ли это вход в сессии
                    if (!_isSessionInitialized)
                    {
                        Debug.Log($"[LanguageChanger] First session launch - using Yandex language: {yandexIndex} ({yandexLanguage})");
                        newIndex = yandexIndex;
                        _isSessionInitialized = true;

                        // Сохраняем язык из Яндекса как базовый
                        PlayerPrefs.SetInt("IndexLanguageSave", newIndex);
                        PlayerPrefs.Save();
                    }
                    else
                    {
                        // Не первый вход - используем сохраненный язык
                        if (PlayerPrefs.HasKey("IndexLanguageSave"))
                        {
                            newIndex = PlayerPrefs.GetInt("IndexLanguageSave", 0);
                            Debug.Log($"[LanguageChanger] Using saved language: {newIndex}");
                        }
                        else
                        {
                            newIndex = yandexIndex;
                        }
                    }
                }
                else
                {
                    // Не Яндекс платформа
                    newIndex = PlayerPrefs.GetInt("IndexLanguageSave", 0);
                    Debug.Log($"[LanguageChanger] Non-Yandex platform, using saved: {newIndex}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[LanguageChanger] Error accessing YG2: {e.Message}");
                newIndex = PlayerPrefs.GetInt("IndexLanguageSave", 0);
            }

            Debug.Log(LocaleSelector.Instance != null);
            // Проверяем, что локаль готова перед применением
            if (LocaleSelector.Instance != null)
            {
                StartCoroutine(ApplyLanguageWhenReady(newIndex));
            }
        }

        private IEnumerator ApplyLanguageWhenReady(int newIndex)
        {
            // Ждем, пока локализация инициализируется
            float timeout = 5f;
            float startTime = Time.time;

            while (!LocaleSelector.Instance.IsLocalizationReady() && Time.time - startTime < timeout)
            {
                yield return new WaitForSeconds(0.1f);
            }

            Debug.Log(newIndex + " " + _index);

            _index = newIndex;
            ApplyLanguage();
            
        }

        private void ApplyLanguage()
        {
            // Скрываем все кнопки языков
            foreach (var button in _buttonsLanguage)
            {
                if (button != null)
                    button.gameObject.SetActive(false);
            }

            // Показываем нужную кнопку
            if (_index >= 0 && _index < _buttonsLanguage.Length && _buttonsLanguage[_index] != null)
            {
                _buttonsLanguage[_index].gameObject.SetActive(true);
            }

            // Меняем локаль
            if (LocaleSelector.Instance != null)
            {
                LocaleSelector.Instance.ChangeLocale(_index);
            }

            Debug.Log($"[LanguageChanger] Applied language: {_index}");
        }

        public void ChooseNextLanguage()
        {
            Debug.Log("[LanguageChanger] Next language!");

            if (_buttonsLanguage[_index] != null)
                _buttonsLanguage[_index].gameObject.SetActive(false);

            if (_index < _buttonsLanguage.Length - 1)
                _index++;
            else
                _index = 0;

            // Сохраняем выбор игрока
            PlayerPrefs.SetInt("IndexLanguageSave", _index);
            PlayerPrefs.Save();

            // Применяем язык
            if (_index >= 0 && _index < _buttonsLanguage.Length && _buttonsLanguage[_index] != null)
                _buttonsLanguage[_index].gameObject.SetActive(true);

            if (LocaleSelector.Instance != null)
                LocaleSelector.Instance.ChangeLocale(_index);

            AudioManager.Instance.PlaySound(_clickSFX);
            Debug.Log($"[LanguageChanger] New language: {_index}");
        }

        public void ChoosePreviousLanguage()
        {
            Debug.Log("[LanguageChanger] Previous language!");

            if (_buttonsLanguage[_index] != null)
                _buttonsLanguage[_index].gameObject.SetActive(false);

            if (_index > 0)
                _index--;
            else
                _index = _buttonsLanguage.Length - 1;

            // Сохраняем выбор игрока
            PlayerPrefs.SetInt("IndexLanguageSave", _index);
            PlayerPrefs.Save();

            // Применяем язык
            if (_index >= 0 && _index < _buttonsLanguage.Length && _buttonsLanguage[_index] != null)
                _buttonsLanguage[_index].gameObject.SetActive(true);

            if (LocaleSelector.Instance != null)
                LocaleSelector.Instance.ChangeLocale(_index);

            AudioManager.Instance.PlaySound(_clickSFX);
        }
    }
}