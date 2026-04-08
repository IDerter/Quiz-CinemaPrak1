using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace QuizCinema
{
    public class LocalizationTable : MonoBehaviour
    {
        [SerializeField] private string tableName = "SkinPanel";
        [SerializeField] private string key = "Simple Guy";

        void Start()
        {
            LocalizationSettings.InitializationOperation.Completed += OnLocalizationInitialized;
        }

        private void OnLocalizationInitialized(AsyncOperationHandle<LocalizationSettings> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log("Localization successfully loaded.");
                var test = LoadLocalizedString(tableName, key);
            }
            else
            {
                Debug.LogError("Failed to initialize localization.");
            }
        }

        private async Task<string> LoadLocalizedString(string tableName, string key)
        {
            var stringTable = LocalizationSettings.StringDatabase.GetTableAsync(tableName);
            var handle = await stringTable.Task;

            if (handle != null)
            {
                var entry = handle.GetEntry(key);
                if (entry != null)
                {
                    return entry.GetLocalizedString();
                }
                else
                {
                    Debug.LogWarning("Key not found: " + key);
                    return string.Empty;
                }
            }
            else
            {
                Debug.LogError("Failed to load the table: " + tableName);
                return string.Empty;
            }
        }
    }
}