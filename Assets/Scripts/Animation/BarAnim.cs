using Spine.Unity;
using System.Collections;
using TowerDefense;
using UnityEngine;
using UnityEngine.UI;

namespace QuizCinema
{
	public class BarAnim : MonoBehaviour
    {
        [SerializeField] private SkeletonGraphic _bar;
        [SerializeField] private SkeletonGraphic _lock;
        [SerializeField] private LockAnim _lockAnim;

        [SerializeField] private bool _isOpen;
        public bool IsOpen { get { return _isOpen; } set { _isOpen = value; } }

        [SerializeField] private string _nameIdle;
        [SerializeField] private string _namePress;

        [SerializeField] private Button _buttonActivLvl;

        [SerializeField] private float _timeDelay = 1f;

        private const string _clickSFX = "ClickSFX";

        private void Start()
        {
            var numberBar = int.Parse(gameObject.name.Substring(gameObject.name.Length - 1));
            _isOpen = MapCompletion.Instance.GetOpensBar[numberBar - 1];
            Debug.Log($"Открыт ли бар с номером {numberBar} - {_isOpen}");
            if (_isOpen)
            {
                //Debug.Log($"Открыт ли бар с номером {numberBar} - {_isOpen}");
                BarActive(numberBar);
            }

            var animSpineArray = _bar.Skeleton.Data.Animations.ToArray();
            foreach (var anim in animSpineArray)
			{
                if (anim.ToString().Contains("idle"))
				{
                    _nameIdle = anim.ToString();
				}
                if (anim.ToString().Contains("press"))
                {
                    _namePress = anim.ToString();
                }
            }
        }

        public void BarInActive()
        {
            if (_bar.Skeleton.Data.FindSkin("locked") != null)
                _bar.Skeleton.SetSkin("locked");

            _bar.AnimationState.SetAnimation(1, "locked", false);

            if (_bar.Skeleton.Data.FindAnimation(_nameIdle) != null)
                _lock.AnimationState.SetAnimation(1, _nameIdle, false);

            _buttonActivLvl.gameObject.SetActive(false);
        }

        public void BarActive(int numberBar)
        {
            if (numberBar != 1)
			{
                if (_bar.Skeleton.Data.FindSkin("unlocked") != null)
                    _bar.Skeleton.SetSkin("unlocked");
                Debug.Log(_bar.Skeleton.Data.FindSkin("unlocked") + " " + numberBar);
                _bar.Skeleton.SetSlotsToSetupPose();
                _bar.LateUpdate();

                _bar.Update(0);
                if (_bar.Skeleton.Data.FindAnimation(_nameIdle) != null)
                    _bar.AnimationState.SetAnimation(1, _nameIdle, true);
                _bar.Skeleton.SetSlotsToSetupPose();
                _bar.LateUpdate();

                _lock.AnimationState.SetAnimation(1, "none", false);

                _buttonActivLvl.gameObject.SetActive(true);
            }
        }

        public void BarPress()
        {
            AudioManager.Instance.PlaySound(_clickSFX);

            _bar.AnimationState.SetAnimation(1, _namePress, false);
        }

        public IEnumerator DelayBarActive()
        {
            yield return new WaitForSeconds(_timeDelay);
            BarOpen();
        }

        public void BarOpen()
        {
            var numberBar = int.Parse(gameObject.name.Substring(gameObject.name.Length - 1));

            if (!MapCompletion.Instance.GetOpensBar[numberBar - 1])
            {
                // Логика для прогрессивной загрузки контента
                if (numberBar > 1) // Начинаем подгрузку только со второго бара
                {
                    int firstLevelToLoad = (numberBar - 1) * 5 + 2;
                    int lastLevelToLoad = numberBar * 5 + 1;
  
                    Debug.Log($"[BarAnim] Bar {numberBar} opened. Preloading levels from {firstLevelToLoad} to {lastLevelToLoad}.");
                    BackgroundDownloader.Instance.EnqueueLevelRange(firstLevelToLoad, lastLevelToLoad);
                }

                BarActive(numberBar);

                _lock.AnimationState.SetAnimation(1, "unlocking", false);
                _isOpen = MapCompletion.Instance.GetOpensBar[numberBar - 1];
                MapCompletion.Instance.GetOpensBar[numberBar - 1] = true;
                MapCompletion.SaveBarProgress();

                //if (_lockAnim != null)
                 //   _lockAnim.LockBar();
            }
			else
			{
                _bar.AnimationState.SetAnimation(1, _nameIdle, true);
            }
        }

        public void ResetProgress()
        {
            PlayerPrefs.SetInt(gameObject.name, 0);
        }
    }
}