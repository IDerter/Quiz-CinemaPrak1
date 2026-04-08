#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using QuizCinema.SO;
using System.Linq;

public class S3ManifestGenerator : EditorWindow
{
    private S3Config s3Config;
    private string outputPath = "Assets/StreamingAssets/image_manifest.json";
    private Vector2 scrollPosition;
    private Dictionary<string, List<string>> previewData = new Dictionary<string, List<string>>();
    private bool isPreviewLoaded = false;
    private string statusMessage = "";

    [MenuItem("Tools/Generate S3 Manifest")]
    public static void ShowWindow()
    {
        GetWindow<S3ManifestGenerator>("S3 Manifest Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("S3 Image Manifest Generator", EditorStyles.boldLabel);
        GUILayout.Space(10);

        s3Config = (S3Config)EditorGUILayout.ObjectField("S3 Config", s3Config, typeof(S3Config), false);

        GUILayout.Space(5);

        outputPath = EditorGUILayout.TextField("Output Path", outputPath);

        if (GUILayout.Button("Browse", GUILayout.Width(100)))
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Manifest", "image_manifest", "json", "Select location for manifest");
            if (!string.IsNullOrEmpty(path))
            {
                outputPath = path;
            }
        }

        GUILayout.Space(10);

        if (s3Config == null)
        {
            EditorGUILayout.HelpBox("Please select S3Config asset", MessageType.Warning);
            return;
        }

        if (GUILayout.Button("Preview Images from S3", GUILayout.Height(30)))
        {
            PreviewImagesFromS3();
        }

        if (isPreviewLoaded)
        {
            GUILayout.Space(10);
            GUILayout.Label($"Preview: {previewData.Values.Sum(list => list.Count)} images found", EditorStyles.boldLabel);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
            foreach (var kvp in previewData)
            {
                GUILayout.Label($"{kvp.Key}: {kvp.Value.Count} images");
            }
            EditorGUILayout.EndScrollView();
        }

        GUILayout.Space(10);

        if (!string.IsNullOrEmpty(statusMessage))
        {
            EditorGUILayout.HelpBox(statusMessage, MessageType.Info);
        }

        if (GUILayout.Button("Generate Manifest from S3", GUILayout.Height(40)))
        {
            GenerateManifestFromS3();
        }
    }

    private async void PreviewImagesFromS3()
    {
        try
        {
            statusMessage = "Connecting to S3...";

            var credentials = new BasicAWSCredentials(s3Config.AccessKey, s3Config.SecretKey);
            var config = new AmazonS3Config
            {
                ServiceURL = s3Config.ServiceURL,
                ForcePathStyle = true,
                UseHttp = true
            };

            using (var client = new AmazonS3Client(credentials, config))
            {
                var request = new ListObjectsV2Request
                {
                    BucketName = s3Config.BucketName,
                    Prefix = s3Config.ImageFolderName.EndsWith("/") ? s3Config.ImageFolderName : s3Config.ImageFolderName + "/",
                    MaxKeys = 1000
                };

                var response = await client.ListObjectsV2Async(request);

                previewData.Clear();

                foreach (var obj in response.S3Objects)
                {
                    if (obj.Key.EndsWith("/")) continue;

                    string fileName = Path.GetFileName(obj.Key);
                    string assetName = Path.GetFileNameWithoutExtension(fileName);

                    // Группируем по первым 2 символам для предпросмотра
                    string prefix = assetName.Length >= 2 ? assetName.Substring(0, 2) : "other";

                    if (!previewData.ContainsKey(prefix))
                        previewData[prefix] = new List<string>();

                    previewData[prefix].Add(assetName);
                }

                isPreviewLoaded = true;
                statusMessage = $"Found {response.S3Objects.Count} images";
            }
        }
        catch (System.Exception e)
        {
            statusMessage = $"Error: {e.Message}";
            Debug.LogError($"Failed to preview S3: {e}");
        }
    }

    private async void GenerateManifestFromS3()
    {
        try
        {
            statusMessage = "Generating manifest from S3...";

            var credentials = new BasicAWSCredentials(s3Config.AccessKey, s3Config.SecretKey);
            var config = new AmazonS3Config
            {
                ServiceURL = s3Config.ServiceURL,
                ForcePathStyle = true,
                UseHttp = true
            };

            var manifest = new ImageManifest
            {
                generated = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                baseUrl = $"{s3Config.ServiceURL}/{s3Config.BucketName}/{s3Config.ImageFolderName}",
                images = new List<ManifestImage>()
            };

            using (var client = new AmazonS3Client(credentials, config))
            {
                string continuationToken = null;
                int totalCount = 0;

                do
                {
                    var request = new ListObjectsV2Request
                    {
                        BucketName = s3Config.BucketName,
                        Prefix = s3Config.ImageFolderName.EndsWith("/") ? s3Config.ImageFolderName : s3Config.ImageFolderName + "/",
                        MaxKeys = 1000,
                        ContinuationToken = continuationToken
                    };

                    var response = await client.ListObjectsV2Async(request);

                    foreach (var obj in response.S3Objects)
                    {
                        if (obj.Key.EndsWith("/")) continue;

                        string fileName = Path.GetFileName(obj.Key);
                        string assetName = Path.GetFileNameWithoutExtension(fileName);
                        string extension = Path.GetExtension(fileName).ToLower();

                        // Исправление 1: преобразуем long? в long с проверкой
                        long size = obj.Size ?? 0; // Если null, ставим 0

                        // Исправление 2: ToString() без аргументов
                        string lastModified = obj.LastModified.HasValue ? obj.LastModified.Value.ToString() : "";

                        manifest.images.Add(new ManifestImage
                        {
                            name = assetName,
                            fileName = fileName,
                            path = obj.Key,
                            extension = extension,
                            size = size,
                            lastModified = lastModified
                        });

                        totalCount++;
                    }

                    continuationToken = response.NextContinuationToken;

                } while (!string.IsNullOrEmpty(continuationToken));

                manifest.totalImages = totalCount;
            }

            // Сохраняем JSON
            string json = JsonUtility.ToJson(manifest, true);
            File.WriteAllText(outputPath, json);

            AssetDatabase.Refresh();

            statusMessage = $"Manifest generated successfully! {manifest.totalImages} images saved to {outputPath}";
            Debug.Log(statusMessage);
        }
        catch (System.Exception e)
        {
            statusMessage = $"Error: {e.Message}";
            Debug.LogError($"Failed to generate manifest: {e}");
        }
    }

    [System.Serializable]
    private class ImageManifest
    {
        public string generated;
        public string baseUrl;
        public int totalImages;
        public List<ManifestImage> images;
    }

    [System.Serializable]
    private class ManifestImage
    {
        public string name;
        public string fileName;
        public string path;
        public string extension;
        public long size;
        public string lastModified;
    }
}
#endif