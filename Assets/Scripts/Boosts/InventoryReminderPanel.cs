using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static QuizCinema.SkinManager;

namespace QuizCinema
{
	public class InventoryReminderPanel : MonoBehaviour
    {
        [SerializeField] private BoostSO[] boosts;
        [SerializeField] private SkinSO[] _skins;
		[SerializeField] private Image _currentSkinImage;
		[SerializeField] private TextMeshProUGUI _textSkinName;
		[SerializeField] private TextMeshProUGUI _textDescriptionSkin;
		[SerializeField] private BoostInGame _skinBooster;

		private const string _clickSFX = "ClickSFX";

		private void Start()
		{
			SkinManager.Instance.OnPutOn += OnPutOn;
			//LoadInfoSkin();
		}

		private void OnDestroy()
		{
			SkinManager.Instance.OnPutOn -= OnPutOn;
		}

		public void LoadInfoSkin()
		{
			SkinSave skin = GetPutOnSkin();
			SetInventoryReminderInfo(skin);
		}

		public void ClickSound()
		{
			AudioManager.Instance.PlaySound(_clickSFX);
		}

		private void OnPutOn(SkinSave skinSave)
		{
			SetInventoryReminderInfo(skinSave);
		}

		private void SetInventoryReminderInfo(SkinSave skinSave)
		{
			foreach (var skin in _skins)
			{
				if (skinSave.SkinName == skin.name)
				{
					_currentSkinImage.sprite = skin.sprite;
					if (_skinBooster.GetSetBoostSO != null && skin.Boost != null)
						_skinBooster.GetSetBoostSO = skin.Boost;

					if (_skinBooster.BoostImage != null && skin.Boost != null)
						_skinBooster.BoostImage.sprite = skin.Boost.sprite;

					_textSkinName.text = LocaleSelector.Instance.LoadLocalizedString("SkinPanel", skin.name).Result;

					_textDescriptionSkin.text = LocaleSelector.Instance.LoadLocalizedString("SkinPanel", skin.DescriptionSkinBooster).Result;
				}
				if (skinSave.SkinName == SkinManager.Instance.DeffaultSkinSO.name)
				{
					_skinBooster.gameObject.SetActive(false);
				}
				else
				{
					_skinBooster.gameObject.SetActive(true);
				}
			}
		}
	}
}