using UnityEngine;
using YG;

namespace QuizCinema
{
	public class LanguageManager : SingletonBase<LanguageManager>
    {
        private const string _clickSFX = "ClickSFX";
        [SerializeField] private bool _isFirst;


		public void Init(Language[] _buttonsLanguage, int _index)
		{
            if (!_isFirst)
            {
                if (YG2.platform == "YandexGames")
                {
                    var currentLanguage = YG2.lang;
                    Debug.Log(currentLanguage + " YANDEX!");

                    if (currentLanguage == "ru")
                    {
                        _index = 1;
                        LocaleSelector.Instance.ChangeLocale(_index);
                        _buttonsLanguage[_index].gameObject.SetActive(true);
                    }
                    else
                    {
                        _index = 0;
                        LocaleSelector.Instance.ChangeLocale(_index);
                        _buttonsLanguage[_index].gameObject.SetActive(true);
                    }
                }
                else
                {
                    Debug.Log("LanguageChanger");

                    _index = PlayerPrefs.GetInt("IndexLanguageSave", 0);
                    _buttonsLanguage[_index].gameObject.SetActive(true);
                    LocaleSelector.Instance.ChangeLocale(_index);
                }
                _isFirst = true;
            }
        }

        public void ChooseNextLanguage(Language[] _buttonsLanguage, int _index)
        {
            Debug.Log("Test!");
            _buttonsLanguage[_index].gameObject.SetActive(false);
            if (_index < _buttonsLanguage.Length - 1)
            {
                _index++;
            }
            else _index = 0;

            LocaleSelector.Instance.ChangeLocale(_index);

            _buttonsLanguage[_index].gameObject.SetActive(true);
            AudioManager.Instance.PlaySound(_clickSFX);

            PlayerPrefs.SetInt("IndexLanguageSave", _index);

            Debug.Log($"ChooseNextLanguage {_index}");
        }

        public void ChoosePreviousLanguage(Language[] _buttonsLanguage, int _index)
        {
            Debug.Log("Previous!");
            _buttonsLanguage[_index].gameObject.SetActive(false);
            if (_index > 0)
            {
                _index--;
            }
            else _index = _buttonsLanguage.Length - 1;

            LocaleSelector.Instance.ChangeLocale(_index);

            _buttonsLanguage[_index].gameObject.SetActive(true);
            AudioManager.Instance.PlaySound(_clickSFX);

            PlayerPrefs.SetInt("IndexLanguageSave", _index);
        }
    }
}