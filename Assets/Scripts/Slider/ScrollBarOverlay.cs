using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QuizCinema
{
    public class ScrollBarOverlay : MonoBehaviour
    {
        [SerializeField] private Scrollbar scrollbar;
        [SerializeField] private GameObject _overlayTop;
        [SerializeField] private GameObject _overlayBottom;
        [SerializeField] private float _prevValue;
        [SerializeField] private bool _startLvl = true;
        [SerializeField] private RectTransform _contentScroll;
        [SerializeField] private int _countScrollBarCallback = 0;
        [SerializeField] private bool _isMove = true;
         
        void Start()
        {
            if (_isMove)
            {
                TrackerDownShop.OnScrollbarStart += OnScrollbarStart;
                TrackerDownShop.OnScrollbarEnd += OnScrollbarEnd;
            }

            scrollbar.onValueChanged.AddListener((float val) => {
                ScrollbarCallback(val);
            });


            if (_contentScroll != null)
                _contentScroll.transform.position = new Vector3(_contentScroll.transform.position.x, -680, _contentScroll.transform.position.z);

            _startLvl = true;
            _countScrollBarCallback = 0;
            _prevValue = 1;
        }

		private void OnDestroy()
		{
            if (_isMove)
            {
                TrackerDownShop.OnScrollbarStart -= OnScrollbarStart;
                TrackerDownShop.OnScrollbarEnd -= OnScrollbarEnd;
            }
        }

		private void OnScrollbarStart()
        {
            scrollbar.onValueChanged.RemoveAllListeners();
            scrollbar.onValueChanged.AddListener((float val) => {
                ScrollbarCallback(val);
            });

            _countScrollBarCallback = 0;
            _startLvl = true;
            _prevValue = 1;

        }

        private void OnScrollbarEnd()
        {

            if (_contentScroll != null)
                _contentScroll.transform.position = new Vector3(_contentScroll.transform.position.x, -680, _contentScroll.transform.position.z);
        }

        private void OnEnable()
		{
            _countScrollBarCallback = 0;
            if (_contentScroll != null)
			    _contentScroll.transform.position = new Vector3(_contentScroll.transform.position.x, -680, _contentScroll.transform.position.z);
            _startLvl = true;
        }


        void ScrollbarCallback(float value)
        {
            if (!_startLvl && _countScrollBarCallback > 2)
			{
                if (_prevValue - value > 0)
                {
                    //_overlayBottom.SetActive(false);
                    //_overlayTop.SetActive(true);
                    if (_overlayTop.TryGetComponent(out FadeImage fadeOverlayTopImage))
                        fadeOverlayTopImage.FadeInStartAnim();
                    if (_overlayBottom.TryGetComponent(out FadeImage fadeOverlayBottomImage))
                        fadeOverlayBottomImage.FadeOutStartAnim();
                }
                else if (_prevValue - value < 0)
                {
                    // _overlayBottom.SetActive(true);
                    // _overlayTop.SetActive(false);
                    if (_overlayTop.TryGetComponent(out FadeImage fadeOverlayTopImage))
                        fadeOverlayTopImage.FadeOutStartAnim();
                    if (_overlayBottom.TryGetComponent(out FadeImage fadeOverlayBottomImage))
                        fadeOverlayBottomImage.FadeInStartAnim();

                }
                else
                {
                    // _overlayBottom.SetActive(false);
                    // _overlayTop.SetActive(false);
                    if (_overlayTop.TryGetComponent(out FadeImage fadeOverlayTopImage))
                        fadeOverlayTopImage.FadeOutStartAnim();
                    if (_overlayBottom.TryGetComponent(out FadeImage fadeOverlayBottomImage))
                        fadeOverlayBottomImage.FadeOutStartAnim();
                }

                _prevValue = value;
            }
            _countScrollBarCallback++;
            _startLvl = false;
        }

        public void ResetState()
		{
            Debug.Log("RESETSTATE SCROLLBAR");
            if (_overlayTop.TryGetComponent(out FadeImage fadeOverlayTopImage))
                fadeOverlayTopImage.FadeOutStartAnim();
            if (_overlayBottom.TryGetComponent(out FadeImage fadeOverlayBottomImage))
                fadeOverlayBottomImage.FadeInStartAnim();
            scrollbar.value = 1;
        }
    }
}