using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace QuizCinema
{
    // Убираем RequireComponent, так как основная логика теперь не на этой картинке
    public class SwipeImage : MonoBehaviour
    {
        [Header("Snapping Logic")]
        [SerializeField] private ScrollRectSnap _scrollRectSnap;

        [Header("UI References")]
        [SerializeField] private Image[] _visulationCadrIndex; // Точки-индикаторы

        [Header("Image Loading Placeholders")]
        [SerializeField] private Sprite _loadingSprite;
        [SerializeField] private Sprite _errorSprite;

        private int _currentIndex = -1; // Начинаем с -1, чтобы гарантировать обновление при первом кадре
        private bool _isInitialized = false;
        
        private const string _swipeSFX = "Swipe"; // Можно оставить, если ScrollRectSnap будет проигрывать звук

        private void Awake()
        {
            AnswersMethods.Instance.OnCreateAnswers += OnCreateAnswers;
            if (_scrollRectSnap != null)
            {
                _scrollRectSnap.OnSnapChangedByUser += PlaySwipeSound;
            }
        }

        private void OnDestroy()
        {
            AnswersMethods.Instance.OnCreateAnswers -= OnCreateAnswers;
            if (_scrollRectSnap != null)
            {
                _scrollRectSnap.OnSnapChangedByUser -= PlaySwipeSound;
            }
        }

        private void Update()
        {
            if (!_isInitialized) return;

            int newIndex = _scrollRectSnap.GetCurrentSnappedIndex();
            if (newIndex != _currentIndex)
            {
                // Если индекс был валидным, делаем старую точку полупрозрачной
                if (_currentIndex >= 0 && _currentIndex < _visulationCadrIndex.Length)
                {
                    SetAlpha(_visulationCadrIndex[_currentIndex], 0.5f);
                }
                
                _currentIndex = newIndex;
                
                // Делаем новую точку непрозрачной
                if (_currentIndex >= 0 && _currentIndex < _visulationCadrIndex.Length)
                {
                    SetAlpha(_visulationCadrIndex[_currentIndex], 1f);
                }
            }
        }

        private void PlaySwipeSound()
        {
            AudioManager.Instance.PlaySound(_swipeSFX);
        }

        private void OnCreateAnswers(Question question)
        {
            _isInitialized = false;
            _currentIndex = -1;

            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(SetupSwipe(question));
            }
        }

        private IEnumerator SetupSwipe(Question question)
        {
            // Даем один кадр, чтобы все дочерние объекты успели создаться
            yield return null; 

            var answerComponents = GetComponentsInChildren<AnswerData>();
            Debug.Log($"[SwipeImage] Found {answerComponents.Length} AnswerData components.");

            if (answerComponents.Length == 0)
            {
                Debug.LogError("[SwipeImage] No AnswerData components found in children. Cannot initialize swipe.");
                yield break;
            }

            // Собираем RectTransform'ы для скроллера
            var itemRects = answerComponents.Select(ans => ans.GetComponent<RectTransform>()).ToArray();
            
            // Инициализируем скроллер
            _scrollRectSnap.Initialize(itemRects);
            _scrollRectSnap.ResetToStart(); // Сбрасываем на начальный элемент

            // Загружаем изображения
            for (int i = 0; i < answerComponents.Length; i++)
            {
                var imageComponent = answerComponents[i].GetComponent<CadrsAnswers>().CurrentImage;
                var imageName = question.Answers[i].InfoList[0];
                
                // Используем гибридную загрузку
                int buildIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
                if (buildIndex <= 2)
                {
                    var spriteFromResources = Resources.Load<Sprite>($"{imageName}");
                    imageComponent.sprite = spriteFromResources ? spriteFromResources : _errorSprite;
                }
                else
                {
                    LoadSpriteForAnswerAsync(imageName, imageComponent);
                }
            }

            // Сбрасываем и устанавливаем начальное состояние индикаторов
            foreach (var indicator in _visulationCadrIndex)
            {
                SetAlpha(indicator, 0.5f);
            }

            _isInitialized = true;
        }
        
        private void SetAlpha(Image image, float alpha)
        {
            if (image == null) return;
            Color color = image.color;
            color.a = alpha;
            image.color = color;
        }

        private async void LoadSpriteForAnswerAsync(string imageName, Image targetImage)
        {
            if (targetImage == null) return;

            targetImage.sprite = _loadingSprite;
            var sprite = await BackgroundDownloader.Instance.GetSpriteAsync(imageName);
            
            if (sprite != null)
            {
                targetImage.sprite = sprite;
            }
            else
            {
                targetImage.sprite = _errorSprite;
                Debug.LogError($"[SwipeImage] ASYNC: Failed to load sprite for asset name: '{imageName}'.");
            }
        }
    }
}