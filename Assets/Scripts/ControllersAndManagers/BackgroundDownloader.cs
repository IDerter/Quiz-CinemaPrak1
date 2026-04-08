using Cysharp.Threading.Tasks;
using QuizCinema.SO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using TowerDefense;

namespace QuizCinema
{
    /// <summary>
    /// Оптимизированный загрузчик для WebGL
    /// - Нет Amazon SDK (несовместим с WebGL)
    /// - Нет Task, только UniTask
    /// - Упрощенное кэширование через PlayerPrefs
    /// - Минимум фоновых операций
    /// </summary>
    public class BackgroundDownloader : SingletonBase<BackgroundDownloader>
    {
        [Header("WebGL Settings")]
        [SerializeField] private string _manifestUrl = "https://your-server.com/image_manifest.json";
        [SerializeField] private string _baseImageUrl = "https://your-s3-url.com/";
        [SerializeField] private int _maxCacheSizeMB = 100;
        [SerializeField] private int _maxConcurrentDownloads = 2;

        [Header("UI References")]
        [SerializeField] private GameObject _networkErrorPanel;

        [Header("Image Quality")]
        [SerializeField] private bool _downscaleOnLowEndDevices = true;
        [SerializeField] private int _maxTextureSize = 2048; // М

        // Публичные данные
        public Dictionary<string, string> ImageUrls { get; private set; } = new Dictionary<string, string>();
        public bool IsMapDataLoaded { get; private set; } = false;
        public bool IsMapLoading { get; private set; } = false;

        public event Action<string> OnDownloadError;

        // Приватные поля
        private Queue<string> _priorityQueue = new Queue<string>();
        private Queue<string> _backgroundQueue = new Queue<string>();
        private HashSet<string> _queuedItems = new HashSet<string>();
        private Dictionary<string, List<string>> _levelAssetMap = new Dictionary<string, List<string>>();

        private int _activeDownloads = 0;
        private bool _isLowEndDevice;
        private bool _isInitialized = false;

        private CancellationTokenSource _mapReloadCts;
        private CancellationTokenSource _internetMonitorCts;
        private DateTime _lastSuccessfulMapLoad = DateTime.MinValue;
        private int _failedReloadAttempts = 0;

        // Константы
        private const float MAP_RELOAD_INTERVAL = 600f; // 10 минут
        private const float MAP_RELOAD_RETRY_INTERVAL = 60f;
        private const string CACHE_PREFIX = "img_cache_";

        protected override void Awake()
        {
            base.Awake();

            // Определяем слабое устройство
            DetectDeviceCapabilities();

            // Запускаем инициализацию
            Initialize();
        }

        public void ReportError(string message)
        {
            Debug.LogError($"[BG Downloader] Error reported: {message}");
            OnDownloadError?.Invoke(message);

            // Показываем панель с ошибкой, если она есть
            if (_networkErrorPanel != null)
            {
                _networkErrorPanel.SetActive(true);
            }
        }

        private void DetectDeviceCapabilities()
        {
            string model = SystemInfo.deviceModel.ToLower();
            _isLowEndDevice = model.Contains("itel") ||
                             model.Contains("infinix") ||
                             model.Contains("tecno") ||
                             SystemInfo.systemMemorySize <= 2048;

            if (_isLowEndDevice)
            {
                Debug.Log("[BG Downloader] LOW-END DEVICE DETECTED");
                QualitySettings.globalTextureMipmapLimit = 2;
                Application.targetFrameRate = 30;
                _maxConcurrentDownloads = 1;
                Application.lowMemory += OnLowMemory;
            }
        }

        private void Initialize()
        {
            if (_isInitialized) return;

            _isInitialized = true;

            // Очищаем кэш при старте
            CleanupCache();

            // Запускаем загрузку карты изображений
            LoadImageMapAsync().Forget();

            // Запускаем рабочие процессы
            StartBackgroundWorkers();
        }

        private void StartBackgroundWorkers()
        {
            _internetMonitorCts = new CancellationTokenSource();
            _mapReloadCts = new CancellationTokenSource();

            MonitorInternetAsync(_internetMonitorCts.Token).Forget();
            ReloadMapPeriodicallyAsync(_mapReloadCts.Token).Forget();
            DownloadWorkerAsync().Forget();
        }

        #region Image Map Loading

        private async UniTask LoadImageMapAsync()
        {
            if (IsMapLoading) return;

            IsMapLoading = true;

            try
            {
                // Пробуем загрузить манифест
                bool success = await TryLoadManifestAsync();

                if (success && ImageUrls.Count > 0)
                {
                    _lastSuccessfulMapLoad = DateTime.UtcNow;
                    IsMapDataLoaded = true;
                    _failedReloadAttempts = 0;

                    Debug.Log($"[BG Downloader] Map loaded: {ImageUrls.Count} images");

                    // Если это первая загрузка, строим каталог ассетов
                    if (_levelAssetMap.Count == 0)
                    {
                        await BuildAssetCatalogsAsync();
                    }
                }
                else
                {
                    IsMapDataLoaded = false;
                    _failedReloadAttempts++;

                    Debug.LogWarning("[BG Downloader] Failed to load image map");
                }
            }
            finally
            {
                IsMapLoading = false;
            }
        }

        private async UniTask<bool> TryLoadManifestAsync()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                return false;
            }

            try
            {
                string manifestPath = _manifestUrl; // Теперь это полный URL

                Debug.Log($"[BG Downloader] Loading manifest from: {manifestPath}");

                using (UnityWebRequest request = UnityWebRequest.Get(manifestPath))
                {
                    request.timeout = 15;
                    request.downloadHandler = new DownloadHandlerBuffer();

                    await request.SendWebRequest();

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        string jsonText = request.downloadHandler.text;

                        var manifest = JsonUtility.FromJson<ImageManifest>(jsonText);

                        if (manifest?.images != null)
                        {
                            ImageUrls.Clear();

                            foreach (var img in manifest.images)
                            {
                                if (!string.IsNullOrEmpty(img.name) && !ImageUrls.ContainsKey(img.name))
                                {
                                    ImageUrls[img.name] = img.path;
                                }
                            }

                            Debug.Log($"[BG Downloader] Manifest loaded: {ImageUrls.Count} images");
                            return true;
                        }
                        else
                        {
                            Debug.LogError($"[BG Downloader] Manifest has no images");
                        }
                    }
                    else
                    {
                        Debug.LogError($"[BG Downloader] Failed to load manifest: {request.error} - URL: {manifestPath}");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[BG Downloader] Manifest error: {e.Message}");
            }

            return false;
        }

        private async UniTask BuildAssetCatalogsAsync()
        {
            Debug.Log("[BG Downloader] Building asset catalogs...");

            // Загружаем каталоги параллельно
            await UniTask.WhenAll(
                BuildAssetCatalogAsync(),
                BuildAssetCatalogClassicAsync()
            );

            Debug.Log($"[BG Downloader] Catalogs built: {_levelAssetMap.Count} levels");

            // После построения каталога, запускаем начальную загрузку
            StartCoroutine(EnqueueInitialAssetsCoroutine());
        }

        private async UniTask BuildAssetCatalogAsync()
        {
            const int maxLevels = 100;

            for (int i = 3; i <= maxLevels; i++)
            {
                string levelKey = $"Q{i}";
                var assets = await ExtractAssetsFromLevelXml(levelKey);

                if (assets != null && assets.Count > 0)
                {
                    _levelAssetMap[levelKey] = assets;
                }
                else
                {
                    // Больше нет уровней
                    break;
                }

                // Даем браузеру передышку
                if (i % 5 == 0)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update);
                }
            }
        }

        private async UniTask BuildAssetCatalogClassicAsync()
        {
            string levelKey = "Q1Classic";
            var assets = await ExtractAssetsFromLevelXml(levelKey);

            if (assets != null && assets.Count > 0)
            {
                _levelAssetMap[levelKey] = assets;
            }
        }

        private async UniTask<List<string>> ExtractAssetsFromLevelXml(string levelKey)
        {
            string xmlFileName = $"{levelKey}.xml";
            string xmlPath;

            // Для разных платформ - разные пути к StreamingAssets
#if UNITY_WEBGL && !UNITY_EDITOR
    // В WebGL сборке - относительный путь (файлы лежат в папке StreamingAssets рядом с игрой)
    xmlPath = Path.Combine(Application.streamingAssetsPath, xmlFileName);
#else
            // В редакторе Unity - нужен file:// протокол для доступа к локальным файлам
            xmlPath = "file://" + Path.Combine(Application.streamingAssetsPath, xmlFileName);
#endif

            //Debug.Log($"[BG Downloader] Loading XML from: {xmlPath}");

            using (UnityWebRequest request = UnityWebRequest.Get(xmlPath))
            {
                request.timeout = 10;

                try
                {
                    await request.SendWebRequest();

                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        Debug.Log($"[BG Downloader] XML not found: {xmlFileName} - {request.error}");
                        return null;
                    }

                    string xmlContent = request.downloadHandler.text;

                    if (string.IsNullOrEmpty(xmlContent))
                    {
                        Debug.LogWarning($"[BG Downloader] XML file is empty: {xmlFileName}");
                        return null;
                    }

                    var assets = new List<string>();

                    // Используем оригинальный подход через Data.Fetch, но в памяти
                    // Создаем временный файл в памяти для Data.Fetch
                    string tempPath = Path.Combine(Application.temporaryCachePath, $"{levelKey}_{Guid.NewGuid()}.xml");

                    try
                    {
                        // Сохраняем XML во временный файл
                        File.WriteAllText(tempPath, xmlContent);

                        // Загружаем через Data.Fetch (как в оригинале)
                        Data levelData = null;

                        // Несколько попыток чтения с задержкой
                        for (int attempt = 0; attempt < 3; attempt++)
                        {
                            try
                            {
                                levelData = Data.Fetch(tempPath);
                                if (levelData != null) break;
                            }
                            catch (IOException) when (attempt < 2)
                            {
                                Thread.Sleep(100 * (attempt + 1));
                            }
                        }

                        if (levelData == null)
                        {
                            Debug.LogError($"[BG Downloader] Failed to parse XML {xmlFileName} with Data.Fetch");
                            return null;
                        }

                        // Извлекаем имена ассетов как в оригинальном BuildAssetCatalog
                        foreach (var question in levelData.Questions)
                        {
                            if (question.IndexPrefab == 0 || question.IndexPrefab == 1)
                            {
                                if (!string.IsNullOrEmpty(question._cadrCinemaName))
                                {
                                    string name = question._cadrCinemaName.Trim();
                                    if (!string.IsNullOrEmpty(name) && !assets.Contains(name))
                                    {
                                        assets.Add(name);
                                       // Debug.Log($"[BG Downloader] Found cadr: {name}");
                                    }
                                }
                            }
                            else if (question.IndexPrefab == 3)
                            {
                                foreach (var answer in question.Answers)
                                {
                                    if (answer.InfoList != null && answer.InfoList.Count > 0 && !string.IsNullOrEmpty(answer.InfoList[0]))
                                    {
                                        string name = answer.InfoList[0].Trim();
                                        if (!string.IsNullOrEmpty(name) && !assets.Contains(name))
                                        {
                                            assets.Add(name);
                                            //Debug.Log($"[BG Downloader] Found answer info: {name}");
                                        }
                                    }
                                }
                            }
                        }
                    }
                    finally
                    {
                        // Всегда удаляем временный файл
                        try
                        {
                            if (File.Exists(tempPath))
                            {
                                File.Delete(tempPath);
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarning($"[BG Downloader] Failed to delete temp file {tempPath}: {e.Message}");
                        }
                    }

                    // Убираем дубликаты
                    assets = assets.Distinct().ToList();

                   // Debug.Log($"[BG Downloader] Found {assets.Count} assets in {xmlFileName}");

                    if (assets.Count > 0)
                    {
                        //Debug.Log($"[BG Downloader] Assets sample: {string.Join(", ", assets.Take(5))}");
                    }

                    return assets;
                }
                catch (Exception e)
                {
                    Debug.LogError($"[BG Downloader] Error parsing {xmlFileName}: {e.Message}");
                    return null;
                }
            }
        }


        #endregion

        #region Download Management

        private async UniTask DownloadWorkerAsync()
        {
            var ct = this.GetCancellationTokenOnDestroy();

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    // Если нет задач - спим
                    if (_priorityQueue.Count == 0 && _backgroundQueue.Count == 0)
                    {
                        await UniTask.Delay(1000, ignoreTimeScale: true, cancellationToken: ct);
                        continue;
                    }

                    // Если достигнут лимит - ждем
                    if (_activeDownloads >= _maxConcurrentDownloads)
                    {
                        await UniTask.Delay(500, ignoreTimeScale: true, cancellationToken: ct);
                        continue;
                    }

                    // Берем следующую задачу
                    string assetName = null;

                    if (_priorityQueue.Count > 0)
                    {
                        assetName = _priorityQueue.Dequeue();
                    }
                    else if (_backgroundQueue.Count > 0)
                    {
                        assetName = _backgroundQueue.Dequeue();
                    }

                    if (!string.IsNullOrEmpty(assetName))
                    {
                        DownloadAssetAsync(assetName).Forget();
                        await UniTask.Delay(100, ignoreTimeScale: true, cancellationToken: ct);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception e)
                {
                    Debug.LogError($"[BG Downloader] Worker error: {e.Message}");
                    await UniTask.Delay(2000, ignoreTimeScale: true, cancellationToken: ct);
                }
            }
        }

        private async UniTask DownloadAssetAsync(string assetName)
        {
            Interlocked.Increment(ref _activeDownloads);

            try
            {
                if (IsAssetCached(assetName))
                {
                    return;
                }

                if (!ImageUrls.TryGetValue(assetName, out string path))
                {
                    Debug.LogWarning($"[BG Downloader] No path for: {assetName}");
                    return;
                }

                string url = _baseImageUrl.TrimEnd('/') + "/" + path.TrimStart('/');

                using (UnityWebRequest request = UnityWebRequest.Get(url))
                {
                    request.timeout = 20;
                    request.downloadHandler = new DownloadHandlerBuffer();

                    await request.SendWebRequest();

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        byte[] data = request.downloadHandler.data;

                        // Проверяем размер и качество
                        Texture2D testTex = new Texture2D(2, 2);
                        if (testTex.LoadImage(data))
                        {
                            Debug.Log($"[BG Downloader] Valid image: {assetName}, Format: {testTex.format}, Size: {testTex.width}x{testTex.height}");

                            // Предупреждаем о потенциальных проблемах с размерами
                            if (testTex.width % 4 != 0 || testTex.height % 4 != 0)
                            {
                                Debug.LogWarning($"[BG Downloader] Image {assetName} has dimensions not multiple of 4 ({testTex.width}x{testTex.height}). May cause compression issues.");
                            }

                            // Проверяем, нужно ли предварительно обработать изображение
                            if (testTex.width > 2048 || testTex.height > 2048)
                            {
                                Debug.Log($"[BG Downloader] Large image detected ({testTex.width}x{testTex.height}), will downscale if needed");
                            }

                            Destroy(testTex);
                            SaveToCache(assetName, data);
                            Debug.Log($"[BG Downloader] Downloaded: {assetName}");
                        }
                        else
                        {
                            Debug.LogError($"[BG Downloader] Invalid image data for {assetName}");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"[BG Downloader] Failed: {assetName} - {request.error}");
                        _priorityQueue.Enqueue(assetName);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[BG Downloader] Download error {assetName}: {e.Message}");
            }
            finally
            {
                _queuedItems.Remove(assetName);
                Interlocked.Decrement(ref _activeDownloads);
            }
        }

        public async UniTask<Sprite> GetSpriteAsync(string assetName)
        {
            if (string.IsNullOrEmpty(assetName)) return null;

            byte[] data = LoadFromCache(assetName);

            if (data == null)
            {
                EnqueueForDownload(new[] { assetName }, true);

                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                try
                {
                    await UniTask.WaitUntil(() => IsAssetCached(assetName),
                        cancellationToken: cts.Token);
                    data = LoadFromCache(assetName);
                }
                catch (OperationCanceledException)
                {
                    Debug.LogWarning($"[BG Downloader] Timeout waiting for: {assetName}");
                    return null;
                }
                finally
                {
                    cts.Dispose();
                }
            }

            if (data == null) return null;

            await UniTask.SwitchToMainThread();

            try
            {
                // Для слабых устройств отключаем mipmaps
                bool useMipMaps = !_isLowEndDevice;

                Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, useMipMaps);

                if (!texture.LoadImage(data))
                {
                    Destroy(texture);
                    return null;
                }

                // Настройки качества
                texture.filterMode = FilterMode.Trilinear; // Лучше чем Bilinear для масштабирования
                texture.anisoLevel = 16; // Максимальное качество
                texture.wrapMode = TextureWrapMode.Clamp;

                if (useMipMaps)
                {
                    texture.mipMapBias = -0.3f; // Делаем мипмапы чуть четче
                }

                texture.Apply(true, true);

                // Высокий Pixels Per Unit для четкости
                float pixelsPerUnit = 200f;

                Sprite sprite = Sprite.Create(texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f),
                    pixelsPerUnit,
                    0,
                    SpriteMeshType.FullRect);

                Debug.Log($"[BG Downloader] Created sprite from {assetName}: {texture.width}x{texture.height}, " +
                         $"format: {texture.format}, PPU: {pixelsPerUnit}, MipMaps: {useMipMaps}");

                return sprite;
            }
            catch (Exception e)
            {
                Debug.LogError($"[BG Downloader] Sprite error: {e.Message}");
                return null;
            }
        }

        private Texture2D ScaleTexture(Texture2D source, float scale)
        {
            int width = Mathf.Max(1, Mathf.RoundToInt(source.width * scale));
            int height = Mathf.Max(1, Mathf.RoundToInt(source.height * scale));

            RenderTexture rt = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
            rt.filterMode = FilterMode.Trilinear;

            Graphics.Blit(source, rt);

            RenderTexture.active = rt;
            Texture2D result = new Texture2D(width, height, TextureFormat.RGBA32, true);
            result.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            result.Apply();

            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);

            return result;
        }

        #endregion

        #region Queue Management

        public void EnqueueForDownload(IEnumerable<string> assetNames, bool isPriority, int startIndex = 0, int endIndex = 0)
        {
            var queue = isPriority ? _priorityQueue : _backgroundQueue;

            var namesToAdd = endIndex > 0
                ? assetNames.Skip(startIndex).Take(endIndex - startIndex)
                : assetNames;

            int added = 0;
            foreach (var name in namesToAdd)
            {
                if (string.IsNullOrEmpty(name)) continue;
                if (_queuedItems.Contains(name)) continue;
                if (!ImageUrls.ContainsKey(name)) continue;
                if (IsAssetCached(name)) continue;

                queue.Enqueue(name);
                _queuedItems.Add(name);
                added++;
            }

            if (added > 0)
            {
                Debug.Log($"[BG Downloader] Added {added} items to {(isPriority ? "priority" : "background")} queue");
            }
        }

        public void EnqueueForPriorityDownload(string levelKey)
        {
            if (_levelAssetMap.TryGetValue(levelKey, out var assets))
            {
                Debug.Log($"EnqueueForDownload - EnqueueForPriorityDownload {levelKey}");
                EnqueueForDownload(assets, true);
            }
        }

        public void EnqueueForPriorityDownloadClassic(string levelKey, int startIndex, int endIndex)
        {
            if (_levelAssetMap.TryGetValue(levelKey, out var assets))
            {
                Debug.Log($"EnqueueForDownload - EnqueueForPriorityDownloadClassic {startIndex} - {endIndex}");
                EnqueueForDownload(assets, true, startIndex, endIndex);
            }
        }

        public void EnqueueLevelRange(int startLevel, int endLevel, bool isPriority = false)
        {
            for (int i = startLevel; i <= endLevel; i++)
            {
                string levelKey = $"Q{i}";
                if (_levelAssetMap.TryGetValue(levelKey, out var assets))
                {
                    EnqueueForDownload(assets, isPriority);
                }
            }
        }

        private IEnumerator EnqueueInitialAssetsCoroutine()
        {
            yield return new WaitUntil(() => MapCompletion.Instance != null);

            var completion = MapCompletion.Instance;
            int lastBar = 0;

            for (int i = 0; i < completion.GetOpensBar.Length; i++)
            {
                if (!completion.GetOpensBar[i]) break;
                lastBar = i;
            }

            int start = lastBar == 0 ? 3 : (lastBar + 1) * 5 + 1;
            int end = lastBar == 0 ? 6 : (lastBar + 2) * 5 + 1;

            EnqueueLevelRange(start, end);
        }

        #endregion

        #region Cache Management (WebGL Optimized)

        private Dictionary<string, byte[]> _memoryCache = new Dictionary<string, byte[]>();
        private Dictionary<string, float> _cacheTimestamps = new Dictionary<string, float>();
        private const float CACHE_DURATION = 300f; // 5 минут

        public bool IsAssetCached(string assetName)
        {
            // Проверяем в памяти
            if (_memoryCache.ContainsKey(assetName))
            {
                // Проверяем, не устарел ли кэш
                if (_cacheTimestamps.TryGetValue(assetName, out float timestamp))
                {
                    if (Time.time - timestamp < CACHE_DURATION)
                    {
                        return true;
                    }
                    else
                    {
                        // Удаляем устаревший кэш
                        _memoryCache.Remove(assetName);
                        _cacheTimestamps.Remove(assetName);
                    }
                }
            }

            // Проверяем в PlayerPrefs как fallback
            string key = CACHE_PREFIX + assetName.GetHashCode();
            return PlayerPrefs.HasKey(key);
        }

        private void SaveToCache(string assetName, byte[] data)
        {
            try
            {
                Debug.Log($"Add to cache {assetName}");
                _memoryCache[assetName] = data;
                _cacheTimestamps[assetName] = Time.time;

#if !UNITY_WEBGL || UNITY_EDITOR
                string key = CACHE_PREFIX + assetName.GetHashCode();
                string base64 = Convert.ToBase64String(data);

                if (base64.Length < 500000)
                {
                    PlayerPrefs.SetString(key, base64);

                    // --- НОВЫЙ КОД: Сохраняем ключ в общий список ---
                    string existingKeys = PlayerPrefs.GetString("cache_keys", "");
                    if (!existingKeys.Contains(key))
                    {
                        string newKeys = string.IsNullOrEmpty(existingKeys) ? key : existingKeys + "," + key;
                        PlayerPrefs.SetString("cache_keys", newKeys);
                        // Сохраняем и время создания для CleanupCache
                        PlayerPrefs.SetString(key + "_time", DateTime.Now.Ticks.ToString());
                    }
                    // ------------------------------------------------

                    PlayerPrefs.Save();
                    Debug.Log($"[BG Downloader] Cached {assetName} ({data.Length / 1024}KB) to PlayerPrefs");
                }
                else
                {
                    Debug.Log($"[BG Downloader] Cached {assetName} ({data.Length / 1024}KB) in memory only");
                }
#else
        Debug.Log($"[BG Downloader] Cached {assetName} ({data.Length / 1024}KB) in memory only (WebGL)");
#endif
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[BG Downloader] Cache save warning: {e.Message}");
            }
        }

        private byte[] LoadFromCache(string assetName)
        {
            // Сначала проверяем в памяти
            if (_memoryCache.TryGetValue(assetName, out byte[] data))
            {
                return data;
            }

            // Затем в PlayerPrefs
            try
            {
                string key = CACHE_PREFIX + assetName.GetHashCode();
                if (PlayerPrefs.HasKey(key))
                {
                    string base64 = PlayerPrefs.GetString(key);
                    data = Convert.FromBase64String(base64);

                    // Также сохраняем в память для быстрого доступа
                    _memoryCache[assetName] = data;
                    _cacheTimestamps[assetName] = Time.time;

                    return data;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[BG Downloader] Cache load error: {e.Message}");
            }

            return null;
        }

        private void CleanupCache()
        {
            try
            {
                // Простая очистка - удаляем старые записи по дате
                // В WebGL PlayerPrefs ограничен, поэтому лучше не хранить слишком много
                var keys = PlayerPrefs.GetString("cache_keys", "").Split(',');
                var now = DateTime.Now;

                foreach (var key in keys)
                {
                    if (string.IsNullOrEmpty(key)) continue;

                    string timeKey = key + "_time";
                    if (PlayerPrefs.HasKey(timeKey))
                    {
                        long ticks = long.Parse(PlayerPrefs.GetString(timeKey));
                        var saveTime = new DateTime(ticks);

                        if ((now - saveTime).TotalDays > 7) // Удаляем старше недели
                        {
                            PlayerPrefs.DeleteKey(key);
                            PlayerPrefs.DeleteKey(timeKey);
                        }
                    }
                }

                PlayerPrefs.Save();
            }
            catch (Exception e)
            {
                Debug.LogError($"[BG Downloader] Cache cleanup error: {e.Message}");
            }
        }

        #endregion

        #region Status Checks

        public bool AreAssetsReadyForLevel(string levelKey)
        {
            if (_levelAssetMap.TryGetValue(levelKey, out var assets))
            {
                List<string> missingAssets = new List<string>();
                List<string> cachedAssets = new List<string>();

                foreach (var asset in assets)
                {
                    if (!IsAssetCached(asset))
                    {
                        missingAssets.Add(asset);
                    }
                    else
                    {
                        cachedAssets.Add(asset);
                    }
                }

                // Логируем статистику по ассетам уровня
                Debug.Log($"[BG Downloader] Level {levelKey} assets stats:");
                Debug.Log($"[BG Downloader]   Total assets needed: {assets.Count}");
                Debug.Log($"[BG Downloader]   Cached assets: {cachedAssets.Count}");
                Debug.Log($"[BG Downloader]   Missing assets: {missingAssets.Count}");

                // Если есть отсутствующие, выводим их списком
                if (missingAssets.Count > 0)
                {
                    Debug.LogWarning($"[BG Downloader] MISSING ASSETS for level {levelKey}:");
                    for (int i = 0; i < missingAssets.Count; i++)
                    {
                        Debug.LogWarning($"[BG Downloader]   [{i + 1}/{missingAssets.Count}] {missingAssets[i]}");

                        // Проверяем, есть ли ассет в манифесте (но не в кэше)
                        if (ImageUrls.ContainsKey(missingAssets[i]))
                        {
                            Debug.Log($"[BG Downloader]     Asset '{missingAssets[i]}' FOUND in manifest but not cached yet");
                        }
                        else
                        {
                            Debug.LogError($"[BG Downloader]     Asset '{missingAssets[i]}' NOT FOUND in manifest! Check manifest or filename");
                        }
                    }

                    return false;
                }

                Debug.Log($"[BG Downloader] All assets ready for level {levelKey} ✓");
                return true;
            }

            Debug.Log($"[BG Downloader] Level {levelKey} not found in asset map, using default Q1/Q2 fallback");
            return levelKey == "Q1" || levelKey == "Q2";
        }

        public bool AreAssetsReadyForClassic(string levelKey, int startIndex, int endIndex)
        {
            if (startIndex >= 0 && endIndex <= 10) return true;

            if (_levelAssetMap.TryGetValue(levelKey, out var assets))
            {
                var checkAssets = assets.Skip(startIndex).Take(endIndex - startIndex);

                foreach (var asset in checkAssets)
                {
                    if (!string.IsNullOrEmpty(asset) && !IsAssetCached(asset))
                    {
                        return false;
                    }
                }
                return true;
            }

            return false;
        }

        #endregion

        #region Background Workers

        private async UniTask MonitorInternetAsync(CancellationToken ct)
        {
            bool lastState = Application.internetReachability != NetworkReachability.NotReachable;

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    await UniTask.Delay(15000, cancellationToken: ct); // Проверка раз в 15 секунд

                    bool currentState = Application.internetReachability != NetworkReachability.NotReachable;

                    if (currentState != lastState)
                    {
                        Debug.Log($"[BG Downloader] Internet changed: {lastState} -> {currentState}");

                        if (currentState && !IsMapDataLoaded)
                        {
                            await LoadImageMapAsync();
                        }

                        lastState = currentState;
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        private async UniTask ReloadMapPeriodicallyAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    await UniTask.Delay((int)(MAP_RELOAD_INTERVAL * 1000), cancellationToken: ct);

                    if (ShouldReloadMap())
                    {
                        await LoadImageMapAsync();
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        private bool ShouldReloadMap()
        {
            if (!IsMapDataLoaded && Application.internetReachability != NetworkReachability.NotReachable)
                return true;

            if ((DateTime.UtcNow - _lastSuccessfulMapLoad) > TimeSpan.FromHours(2))
                return true;

            return false;
        }

        #endregion

        private void OnLowMemory()
        {
            Debug.LogWarning("[BG Downloader] Low memory!");

            // Очищаем очереди
            _priorityQueue.Clear();
            _backgroundQueue.Clear();
            _queuedItems.Clear();

            // Очищаем кэш PlayerPrefs (оставляем только критическое)
            var keys = PlayerPrefs.GetString("cache_keys", "").Split(',');
            int keepCount = Mathf.Max(1, keys.Length / 2);

            for (int i = 0; i < keys.Length; i++)
            {
                if (i >= keepCount && !string.IsNullOrEmpty(keys[i]))
                {
                    PlayerPrefs.DeleteKey(keys[i]);
                    PlayerPrefs.DeleteKey(keys[i] + "_time");
                }
            }

            PlayerPrefs.Save();
            Resources.UnloadUnusedAssets();
        }

        private void OnDestroy()
        {
            _mapReloadCts?.Cancel();
            _mapReloadCts?.Dispose();

            _internetMonitorCts?.Cancel();
            _internetMonitorCts?.Dispose();

            if (_isLowEndDevice)
            {
                Application.lowMemory -= OnLowMemory;
            }
        }

        #region Editor Tools

        // Этот метод появится при клике правой кнопкой мыши по компоненту в инспекторе
        [ContextMenu("Clear Image Cache")]
        public void ClearCacheFromInspector()
        {
            // 1. Очищаем оперативную память (если игра запущена)
            _memoryCache?.Clear();
            _cacheTimestamps?.Clear();
            _queuedItems?.Clear();
            _priorityQueue?.Clear();
            _backgroundQueue?.Clear();

            // 2. Очищаем PlayerPrefs
            ClearPlayerPrefsCache();

            Debug.Log("[BG Downloader] Memory and PlayerPrefs cache cleared via Inspector!");
        }

        // Общая логика очистки PlayerPrefs
        private static void ClearPlayerPrefsCache()
        {
            string keysStr = PlayerPrefs.GetString("cache_keys", "");
            if (!string.IsNullOrEmpty(keysStr))
            {
                var keys = keysStr.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                int deletedCount = 0;

                foreach (var key in keys)
                {
                    PlayerPrefs.DeleteKey(key);
                    PlayerPrefs.DeleteKey(key + "_time");
                    deletedCount++;
                }

                PlayerPrefs.DeleteKey("cache_keys");
                PlayerPrefs.Save();

                Debug.Log($"[BG Downloader] Deleted {deletedCount} items from PlayerPrefs.");
            }
            else
            {
                Debug.Log("[BG Downloader] PlayerPrefs cache is already empty.");
            }
        }

#if UNITY_EDITOR
        // Этот метод появится в верхней панели Unity: Tools -> QuizCinema -> Clear Image Cache
        [UnityEditor.MenuItem("Tools/QuizCinema/Clear Image Cache")]
        public static void ClearCacheFromMenu()
        {
            ClearPlayerPrefsCache();
        }
#endif

        #endregion
    }

    // Класс для манифеста
    [System.Serializable]
    public class ImageManifest
    {
        public string generated;
        public List<ManifestImage> images;
    }

    [System.Serializable]
    public class ManifestImage
    {
        public string name;
        public string fileName;
        public string path;
        public string extension;
    }
}