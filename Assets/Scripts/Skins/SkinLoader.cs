using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

namespace QuizCinema
{
	public class SkinLoader : SingletonBase<SkinLoader>
    {
        [SerializeField] private GameObject[][] _buttons; // index 0 - unbuy, 1 - put on, 2 - take off
        [SerializedDictionary("Name skin", "Button")]
        public SerializedDictionary<SkinSO, GameObject[]> DictionarySkinButtons;
        [SerializedDictionary] public Dictionary<SkinSO, GameObject[]> skinButtons; // index 0 - unbuy, 1 - put on, 2 - take off


		private void Start()
		{
            LoadSkinsSave();
		}

		public void LoadSkinsSave()
        {
            var skinList = SkinManager.Instance.MainSave;

            for (int i = 0; i < skinList.Length; i++)
            {
                foreach (var elem in DictionarySkinButtons.Keys)
				{
                    if (skinList[i].SkinName == elem.name.ToString())
					{
                        var buttonGreen = DictionarySkinButtons[elem].GetValue(0) as GameObject;
                        var buttonPutOn = DictionarySkinButtons[elem].GetValue(1) as GameObject;
                        var buttonTakeOff = DictionarySkinButtons[elem].GetValue(2) as GameObject;
                        var viewSelect = DictionarySkinButtons[elem]?.GetValue(3) as GameObject;
                        
                        if (skinList[i].Buy && skinList[i].PutOn == false)
						{
							buttonPutOn.SetActive(true);
							buttonGreen.SetActive(false);
							buttonTakeOff.SetActive(false);
                            if (viewSelect != null)
                                viewSelect.SetActive(false);
                        }
                        if (!skinList[i].Buy)
                        {
							buttonPutOn.SetActive(false);
							buttonGreen.SetActive(true);
							buttonTakeOff.SetActive(false);
                            if (viewSelect != null)
                                viewSelect.SetActive(false);
                        }
                        if (skinList[i].Buy && skinList[i].PutOn == true)
                        {
							buttonPutOn.SetActive(false);
							buttonGreen.SetActive(false);
							buttonTakeOff.SetActive(true);
                            if (viewSelect != null)
                                viewSelect.SetActive(true);
                        }
                    }
				}
             }
        }

        public void UpdateSkinSave(SkinManager.SkinSave skinSave)
        {
            foreach (var elem in DictionarySkinButtons.Keys)
            {
                if (skinSave.SkinName == elem.name.ToString())
                {
                    var buttonGreen = DictionarySkinButtons[elem].GetValue(0) as GameObject;
                    var buttonPutOn = DictionarySkinButtons[elem].GetValue(1) as GameObject;
                    var buttonTakeOff = DictionarySkinButtons[elem].GetValue(2) as GameObject;
                    var viewSelect = DictionarySkinButtons[elem]?.GetValue(3) as GameObject;

                    if (skinSave.Buy && skinSave.PutOn == false && buttonGreen.activeSelf)
                    {
                        buttonGreen.GetComponent<ClickAsyncAnim>().ClickAnimBuy(false);

                    }
                    else if (!skinSave.Buy)
                    {
                        //buttonGreen.GetComponent<ClickAsyncAnim>().ClickAnim(false);

                    }
                    else if (skinSave.Buy && skinSave.PutOn == true)
                    {
                        buttonTakeOff.GetComponent<ClickAsyncAnim>().ClickAnimBuy(true);
                        viewSelect.SetActive(true);
                    }

                    else if (skinSave.Buy && skinSave.PutOn == false && buttonGreen.activeSelf == false)
                    {
                        buttonPutOn.GetComponent<ClickAsyncAnim>().ClickAnimBuy(false);
                        viewSelect.SetActive(false);
                    }
                }
            }
            
        }
    }
}