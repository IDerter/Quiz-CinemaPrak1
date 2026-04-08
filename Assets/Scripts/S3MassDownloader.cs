using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using QuizCinema.SO; // Ваша папка с конфигом S3

namespace QuizCinema
{
    public class S3MassDownloader : MonoBehaviour
    {
        private string _targetFolderPath;

        [ContextMenu("Запустить полную выгрузку S3")]
        public async void StartFullExport()
        {
            // 1. Настройка путей
            // Сохраняем в папку "S3_Export" на рабочем столе или в проекте
            _targetFolderPath = Path.Combine(Application.dataPath, "../S3_Full_Export");

            if (!Directory.Exists(_targetFolderPath))
                Directory.CreateDirectory(_targetFolderPath);

            Debug.Log($"<color=green>Старт выгрузки!</color> Файлы будут здесь: {_targetFolderPath}");

            // 2. Инициализация клиента
            var s3Config = Resources.Load<S3Config>("S3Config");
            var credentials = new BasicAWSCredentials(s3Config.AccessKey, s3Config.SecretKey);
            var config = new AmazonS3Config
            {
                ServiceURL = s3Config.ServiceURL,
                ForcePathStyle = true
            };
            var s3Client = new AmazonS3Client(credentials, config);

            try
            {
                // 3. Получение списка ВСЕХ объектов
                var listRequest = new ListObjectsV2Request
                {
                    BucketName = s3Config.BucketName,
                    Prefix = s3Config.ImageFolderName.EndsWith("/") ? s3Config.ImageFolderName : s3Config.ImageFolderName + "/"
                };

                ListObjectsV2Response response;
                int totalDownloaded = 0;

                do
                {
                    response = await s3Client.ListObjectsV2Async(listRequest);

                    foreach (S3Object obj in response.S3Objects)
                    {
                        if (obj.Key.EndsWith("/")) continue; // Пропускаем папки

                        // 4. Загрузка каждого файла
                        await DownloadFile(s3Client, s3Config.BucketName, obj.Key);
                        totalDownloaded++;
                        Debug.Log($"Загружено: {totalDownloaded} - {obj.Key}");
                    }

                    listRequest.ContinuationToken = response.NextContinuationToken;
                } while (response.IsTruncated == true);

                Debug.Log($"<color=cyan>ГОТОВО!</color> Всего выкачано файлов: {totalDownloaded}");
                // Открываем папку в проводнике по завершении
                Application.OpenURL("file://" + _targetFolderPath);
            }
            catch (Exception e)
            {
                Debug.LogError($"Ошибка при массовой выгрузке: {e.Message}");
            }
        }

        private async Task DownloadFile(IAmazonS3 client, string bucket, string key)
        {
            var getRequest = new GetObjectRequest
            {
                BucketName = bucket,
                Key = key
            };

            using (var response = await client.GetObjectAsync(getRequest))
            {
                // Сохраняем с оригинальным именем файла (а не хешем, как в игре)
                string fileName = Path.GetFileName(key);
                string destination = Path.Combine(_targetFolderPath, fileName);

                await response.WriteResponseStreamToFileAsync(destination, false, default);
            }
        }
    }
}