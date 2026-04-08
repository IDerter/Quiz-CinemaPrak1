using System.Collections;
using UnityEngine;
using YG;

namespace QuizCinema
{
    public class InAppShow : MonoBehaviour
    {
        [Header("UI References")]
        [Tooltip("Префаб или панель с предложением покупки монет")]
        [SerializeField] private GameObject[] _inAppPromoPanel;
        [SerializeField] private GameObject[] _thiefSkins;

        [Header("Editor Testing")]
        [Tooltip("Если включено, панель будет показываться в Unity Editor так, словно игра запущена на Яндекс.Играх")]
        [SerializeField] private bool _simulateYandexInEditor = true;

        private void Start()
        {
            // На старте принудительно скрываем панель
            foreach (var promo in _inAppPromoPanel)
			{
                promo.SetActive(false);
			}

            // Начинаем процесс проверки платформы
            StartCoroutine(SetupPlatformUIWhenReady());
        }

        private IEnumerator SetupPlatformUIWhenReady()
        {
            float timeout = 10f;
            float elapsed = 0f;

            // Ждем инициализацию YG2
            while (!IsYandexReady() && elapsed < timeout)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (IsYandexReady())
            {
                // Реальная проверка платформы (для финального билда)
                bool isYandexGames = !string.IsNullOrEmpty(YG2.platform) && YG2.platform.Contains("Yandex");

                // --- БЛОК ДЛЯ ТЕСТИРОВАНИЯ В РЕДАКТОРЕ ---
#if UNITY_EDITOR
                if (_simulateYandexInEditor)
                {
                    isYandexGames = true;
                    Debug.Log("<color=yellow>[InAppShow] СИМУЛЯЦИЯ: Принудительный показ In-App панели в Editor</color>");
                }
#endif
                // ----------------------------------------
                Debug.Log($"IsYandexGames {isYandexGames}");
                if (_inAppPromoPanel != null)
                {
                    foreach (var promo in _inAppPromoPanel)
                    {
                        promo?.SetActive(isYandexGames);
                    }
                    if (_thiefSkins.Length == 2)
                    {
                        _thiefSkins[0]?.SetActive(isYandexGames);
                        _thiefSkins[1]?.SetActive(!isYandexGames);
                    }
                    //_inAppPromoPanel.SetActive(isYandexGames);
                }

                Debug.Log($"[InAppShow] Platform check complete. Platform: {YG2.platform}, Promo Active: {isYandexGames}");
            }
            else
            {
                Debug.LogWarning("[InAppShow] Yandex SDK initialization timeout. Promo panel remains hidden.");
            }
        }

        private bool IsYandexReady()
        {
            try
            {
                if (!string.IsNullOrEmpty(YG2.platform))
                {
                    if (!string.IsNullOrEmpty(YG2.lang))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }

            return false;
        }
    }
}