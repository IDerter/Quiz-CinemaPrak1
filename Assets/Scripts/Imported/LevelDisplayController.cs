using QuizCinema;
using Spine.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TowerDefense
{
	public enum TypeStarts
    {
        Bar,
        Lvl
    }

    public class LevelDisplayController : MonoBehaviour
    {
        [SerializeField] private MapLevel[] _levels;
        [SerializeField] private BranchLevel[] _branchLevels;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private RectTransform _contentRectTransform;
#if UNITY_ANDROID
        [SerializeField] private RateUsManager _rateUsManager;
#endif

        private void Start()
        {
            var drawLevel = 0;
            var value = 1;
            MapLevel lastOpenedBar = null;
            int openedBarsCount = 0;

            while (value != 0 && drawLevel < _levels.Length)
            {
                value = _levels[drawLevel].Initialize();
                Debug.Log($"Level '{_levels[drawLevel].name}' initialized with {value} stars.");

                if (_levels[drawLevel].GetType == TypeStarts.Bar)
                {
                    lastOpenedBar = _levels[drawLevel];
                    openedBarsCount++;
#if UNITY_ANDROID
                    if (openedBarsCount == 4)
                    {
                        if (_rateUsManager != null)
                            _rateUsManager.ShowReview();
                        else
                            Debug.LogWarning("RateUsManager is not assigned in the inspector.");
                    }
#endif
                }

                ActivateBarAndLvl(true, drawLevel);

                drawLevel += 1;
            }

            for (int i = drawLevel; i < _levels.Length; i++)
            {
                // _levels[i].gameObject.SetActive(false);
                ActivateBarAndLvl(false, i);
            }

            for (int i = 0; i < _branchLevels.Length; i++)
            {
                _branchLevels[i].TryActivate();
            }

            if (lastOpenedBar != null)
            {
                int totalBarsCount = 0;
                foreach (var level in _levels)
                {
                    if (level.GetType == TypeStarts.Bar)
                    {
                        totalBarsCount++;
                    }
                }
                
                Debug.Log($"lastOpenedBar != null {lastOpenedBar.gameObject.name} openedBars {openedBarsCount} totalBars {totalBarsCount}");
                ScrollToLastOpenedBar(openedBarsCount, totalBarsCount);
            }
        }

        private void ScrollToLastOpenedBar(int openedBars, int totalBars)
        {
            if (_scrollRect == null)
            {
                Debug.LogWarning("ScrollRect is not assigned.");
                return;
            }

            // We need to wait for the end of the frame for the layout to be updated correctly.
            StartCoroutine(ScrollCoroutine(openedBars, totalBars));
        }

        private System.Collections.IEnumerator ScrollCoroutine(int openedBars, int totalBars)
        {
            // Wait for the end of the frame to ensure all UI elements have been placed.
            yield return new WaitForEndOfFrame();

            float normalizedPosition = 0f;
            if (totalBars > 1 && openedBars > 1)
            {
                // According to the formula: 1 / totalBars * openedBars
                // But since we want to position based on the number of bars,
                // we should consider the spaces between them.
                // If we have 5 bars, we have 4 "segments" of scrolling.
                // When 2nd bar is open, we should be at 1/4th of the way.
                // When 5th bar is open, we should be at 4/4th = 1 (top).
                normalizedPosition = (float)(openedBars - 1) / (totalBars - 1);
            }
            

            _scrollRect.verticalNormalizedPosition = Mathf.Clamp01(normalizedPosition);
            
            Debug.Log($"Scrolling to opened bar {openedBars}/{totalBars}. Normalized Position: {normalizedPosition}");
        }
      
        public void LoadLevel(int index)
        {
            SceneManager.LoadScene(index);
        }

        private void ActivateBarAndLvl(bool activate, int index)
        {
            _levels[index].Lock = PlayerPrefs.GetInt(_levels[index].name) == 1 ? false : true;

            if (activate)
            {
                if (_levels[index].BarAnim != null && _levels[index].GetType == TypeStarts.Bar)
                {
                    _levels[index].BarAnim.IsOpen = true;
                    _levels[index].StartCoroutine(_levels[index].BarAnim.DelayBarActive());
                }
                if (_levels[index].GetType == TypeStarts.Lvl && _levels[index].Lock)
                {
                    if (_levels[index].LockAnim != null)
                    {
                        _levels[index].LockAnim.AnimationState.SetAnimation(1, "unlocking", false);
                        Destroy(_levels[index].LockAnim.gameObject, 3f);
                    }
                    if (_levels[index].OverlayImage.TryGetComponent(out FadeImage fadeOverlay))
                    {
                        fadeOverlay.FadeOutStartAnim();
                        fadeOverlay.GetComponent<Image>().raycastTarget = false;
                        Destroy(fadeOverlay.gameObject, 3f);
                    }
                    else if(_levels[index].OverlayImage != null)
                    {
                        _levels[index].OverlayImage.gameObject.SetActive(false);
                    }

                    if (_levels[index].GetLockImage != null)
                    {
                        _levels[index].GetLockImage.gameObject.SetActive(false);
                    }

                    _levels[index].ButtonLoadScene.SetActive(true);
                    PlayerPrefs.SetInt(_levels[index].name, 1);
                    _levels[index].Lock = false; // Directly set to false
                }
				else
				{
                    if (_levels[index].GetType == TypeStarts.Lvl)
					{
                        _levels[index].LockAnim?.gameObject.SetActive(false);
                        _levels[index].GetLockImage?.gameObject.SetActive(false);
                        _levels[index].OverlayImage?.gameObject.SetActive(false);
                        _levels[index].ButtonLoadScene.SetActive(true);
                    }
				}
            }
            else
            {
                if (_levels[index].BarAnim != null && _levels[index].GetType == TypeStarts.Bar)
                {
                  //  _levels[index].BarAnim.IsOpen = false;
                  //  _levels[index].BarAnim.BarInActive();
                }

                if (_levels[index].GetType == TypeStarts.Lvl && PlayerPrefs.GetInt(_levels[index].name) == 0)
                {
                    _levels[index].GetLockImage?.gameObject.SetActive(true);
                    _levels[index].OverlayImage?.gameObject.SetActive(true);
                }
            }

            
         /*   if (_levels[index].LockAnim != null)
                _levels[index].LockAnim.AnimationState.SetAnimation(1, "unlocking", false);

            else _levels[index].GetLockImage?.gameObject.SetActive(!activate);

            _levels[index].OverlayImage?.gameObject.SetActive(!activate);
         */
        }

        public void DeleteLockAndOverlay(int index)
		{
            Destroy(_levels[index].LockAnim?.gameObject, 1f);
            Destroy(_levels[index].OverlayImage?.gameObject, 1f);
        }

        public void SetPressAnimLock(SkeletonGraphic lockObj)
		{
            lockObj.AnimationState.SetAnimation(1, "press", false);

            AudioManager.Instance.PlaySound("Lock");
        }
    }
}