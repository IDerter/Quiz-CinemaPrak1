using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization.Settings;
using YG;

namespace QuizCinema
{
    public class LocaleSelector : SingletonBase<LocaleSelector>
    {
        public event Action OnChangeLangugage;

        [SerializeField] private bool _active;
        [SerializeField] private int _localeId;
        [SerializeField] private string tableName = "SkinPanel";
        [SerializeField] private string key = "Simple Guy";

        private bool _isInitialized = false;

        protected override void Awake()
        {
            base.Awake();

            LocalizationSettings.InitializationOperation.Completed += _ =>
            {
                _isInitialized = true;
                Debug.Log("[LocaleSelector] Localization initialized");
            };
        }

        public bool IsLocalizationReady()
        {
            return _isInitialized && LocalizationSettings.AvailableLocales != null
                   && LocalizationSettings.AvailableLocales.Locales.Count > 0;
        }

        public void ChangeLocale(int localeID)
        {
            Debug.Log($"[LocaleSelector] ChangeLocale to {localeID}");

            if (!IsLocalizationReady())
            {
                Debug.LogWarning("[LocaleSelector] Localization not ready yet, waiting...");
                StartCoroutine(ChangeLocaleWhenReady(localeID));
                return;
            }

            if (localeID < 0 || localeID >= LocalizationSettings.AvailableLocales.Locales.Count)
            {
                Debug.LogError($"[LocaleSelector] Invalid locale ID: {localeID}");
                return;
            }

            SetLocale(localeID);
        }

        private IEnumerator ChangeLocaleWhenReady(int localeID)
        {
            float timeout = 5f;
            float startTime = Time.time;

            while (!IsLocalizationReady() && Time.time - startTime < timeout)
            {
                yield return new WaitForSeconds(0.1f);
            }

            if (IsLocalizationReady())
            {
                ChangeLocale(localeID);
            }
            else
            {
                Debug.LogError("[LocaleSelector] Timeout waiting for localization");
            }
        }

        public int GetLocale()
        {
            return _localeId;
        }

        public void SetLocale(int localeID)
        {
            if (_active) return;

            _active = true;
            _localeId = localeID;

            try
            {
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeID];
                Debug.Log($"[LocaleSelector] Language set to: {LocalizationSettings.SelectedLocale.LocaleName}");
                OnChangeLangugage?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError($"[LocaleSelector] Error: {e.Message}");
            }

            _active = false;
        }

        // АСИНХРОННЫЙ МЕТОД (рекомендуется)
        public async Task<string> LoadLocalizedString(string tableName, string key)
        {
            try
            {
                // Ждем инициализацию локализации
                if (!IsLocalizationReady())
                {
                    Debug.Log($"[LocaleSelector] Waiting for localization before loading key: {key}");
                    await WaitForLocalizationAsync();
                }

                // Загружаем таблицу
                var tableOperation = LocalizationSettings.StringDatabase.GetTableAsync(tableName);
                await tableOperation.Task;

                if (tableOperation.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                {
                    var table = tableOperation.Result;
                    if (table != null)
                    {
                        var entry = table.GetEntry(key);
                        if (entry != null)
                        {
                            string localizedText = entry.GetLocalizedString();
                            Debug.Log($"[LocaleSelector] Loaded '{key}' = '{localizedText}'");
                            return localizedText;
                        }
                        else
                        {
                            Debug.LogWarning($"[LocaleSelector] Key '{key}' not found in table '{tableName}'");
                            return key; // Возвращаем ключ, если не нашли
                        }
                    }
                }

                Debug.LogWarning($"[LocaleSelector] Failed to load table '{tableName}'");
                return key;
            }
            catch (Exception e)
            {
                Debug.LogError($"[LocaleSelector] Error loading string: {e.Message}");
                return key;
            }
        }

        // СИНХРОННЫЙ МЕТОД (для совместимости с вашим кодом)
        public string LoadLocalizedStringSync(string tableName, string key)
        {
            try
            {
                if (!IsLocalizationReady())
                {
                    Debug.LogWarning($"[LocaleSelector] Localization not ready, returning key: {key}");
                    return key;
                }

                var table = LocalizationSettings.StringDatabase.GetTable(tableName);
                if (table != null)
                {
                    var entry = table.GetEntry(key);
                    if (entry != null)
                    {
                        return entry.GetLocalizedString();
                    }
                }

                return key;
            }
            catch (Exception e)
            {
                Debug.LogError($"[LocaleSelector] Error: {e.Message}");
                return key;
            }
        }

        private async Task WaitForLocalizationAsync()
        {
            float timeout = 5f;
            float startTime = Time.time;

            while (!IsLocalizationReady() && Time.time - startTime < timeout)
            {
                await Task.Delay(100);
            }
        }
    }
}