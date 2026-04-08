using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace QuizCinema
{
	public class ClickAsyncAnim : MonoBehaviour
    {
        [SerializeField] private RectTransform _objAnim;
        private Tween objTween;
        [SerializeField] private GameObject _viewGreenBuySkin;
        [SerializeField] private GameObject _viewBluePutOn;
        [SerializeField] private GameObject _viewBlueTakeOff;

        [SerializeField] private GameObject _openObject;
        [SerializeField] private GameObject _closeObject;
        [SerializeField] private bool _isDisable;
        [SerializeField] private bool _isLoop;

        private const string _clickSFX = "ClickSFX";

        private void Start()
        {
            _objAnim = GetComponent<RectTransform>();
            if (_isLoop)
			{
                if (TryGetComponent(out Animator anim))
				{
                    Debug.Log("Animator test!");
                    anim.enabled = true;
				}
			}
        }

		[Button()]
        public virtual async void ClickAnim()
        {
            //objTween = _objAnim.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 1f).SetEase(Ease.InOutElastic);
            AudioManager.Instance.PlaySound(_clickSFX);

            objTween = _objAnim.DOScale(new Vector3(0.95f, 0.95f, 0.95f), 0.25f);
            await objTween.ToUniTask();

            objTween = _objAnim.DOScale(new Vector3(1f, 1f, 1f), 0.25f);
            await objTween.ToUniTask();

            if (_openObject != null)
                _openObject.SetActive(true);
            if (_closeObject != null)
                _closeObject.SetActive(false);
            if (_isDisable)
                gameObject.transform.parent.gameObject.SetActive(false);
        }

        public async void ClickAnimBuy(bool putOn)
        {
            //objTween = _objAnim.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 1f).SetEase(Ease.InOutElastic);
            
            objTween = _objAnim.DOScale(new Vector3(0.95f, 0.95f, 0.95f), 0.25f);
            await objTween.ToUniTask();

            objTween = _objAnim.DOScale(new Vector3(1f, 1f, 1f), 0.25f);
            await objTween.ToUniTask();
            

            if (putOn)
                await TakeOffShow();
            else
                await PutOnShow();

        }

        public async UniTask PutOnShow()
		{
            Debug.Log("Put On Show");
            if (objTween != null)
                await objTween.ToUniTask();

            _viewGreenBuySkin.SetActive(false);
            _viewBluePutOn.SetActive(true);
            _viewBlueTakeOff.SetActive(false);
        }

        public async UniTask TakeOffShow()
        {
            Debug.Log("Take Off Show");
            if (objTween != null)
                await objTween.ToUniTask();

            _viewGreenBuySkin.SetActive(false);
            _viewBluePutOn.SetActive(false);
            _viewBlueTakeOff.SetActive(true);
        }
    }
}
