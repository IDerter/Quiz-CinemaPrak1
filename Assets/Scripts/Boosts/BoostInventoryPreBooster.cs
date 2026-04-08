using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TowerDefense;
using UnityEngine;
using static QuizCinema.InterfaceBoost;

namespace QuizCinema
{
    public class BoostInventoryPreBooster : SingletonBase<BoostInventoryPreBooster>, IBoostInventory
    {
        public event Action OnSaveListBoosts;
        public event Action<BoostUICount> OnPressButtonBoostInventory;

        [SerializeField] private BoostUICount[] _listBoosts = new BoostUICount[3];
        public BoostUICount[] ListBoosts { get { return _listBoosts; } set { _listBoosts = value; } }
        [SerializeField] private BoostSlot[] _listInventoryBoosts;

        [SerializeField] private BoostUICount[] _boostsActive;
        [SerializeField] private TextMeshProUGUI _textDescription;


        private int _lenListBoosts = 3;

        protected override void Awake()
        {
            base.Awake();

        }

        IEnumerator LoadInventoryDelay()
        {
            yield return new WaitForSeconds(0.0000005f);

            LoadInventory();
        }

        private void OnEnable()
		{
            for (int i = 0; i < _listBoosts.Length; i++)
            {
                _listBoosts[i] = null;
                _listInventoryBoosts[i].BoostImage.sprite = _listInventoryBoosts[i].DefaultSprite;
            }

            Debug.Log("In BoostInventory LoadInventory - Start Method");
            StartCoroutine(LoadInventoryDelay());

            for (int i = 0; i < _listBoosts.Length; i++)
            {
                if (_listBoosts[i] != null)
                {
                    _listInventoryBoosts[i].GetSetBoostSO = _listBoosts[i].GetSetBoostSO;
                }
            }
        }

		public void FillList(BoostUICount inventoryBoost)
        {
            for (int i = 0; i < _listBoosts.Length; i++)
            {
                if (_listBoosts[i] == null && BoostsManager.GetCountBoost(inventoryBoost.GetSetBoostSO) > 0)
                {
                    _listBoosts[i] = inventoryBoost;

                    BoostsManager.Instance.MainSaveListBoosts._listBoosts[i] = inventoryBoost.GetSetBoostSO.name.ToString();
                    BoostsManager.Instance.SaveListBoosts._listBoosts[i] = inventoryBoost.GetSetBoostSO.name.ToString(); ;
                    //_saveBoosts._listBoosts[i] = inventoryBoost.GetSetBoostSO.name.ToString();
                    _listInventoryBoosts[i].GetSetBoostSO = inventoryBoost.GetSetBoostSO;
                    _listInventoryBoosts[i].BoostImage.sprite = inventoryBoost.BoostImage.sprite;
                    _listInventoryBoosts[i].WarningImage.gameObject.SetActive(false);

                    Debug.Log(inventoryBoost.BoostImage.name);
                    Debug.Log(_listInventoryBoosts[i].BoostImage.name);

                    BoostsManager.TakeFromInventory(_listInventoryBoosts[i].GetSetBoostSO, _listBoosts);
                    OnSaveListBoosts?.Invoke();
                    OnPressButtonBoostInventory?.Invoke(inventoryBoost);

                    break;
                }
            }
        }

        private void LoadSlotValues(BoostUICount inventoryBoost)
        {
            for (int i = 0; i < _listBoosts.Length; i++)
            {
                if (_listBoosts[i] == null && BoostsManager.GetCountBoost(inventoryBoost.GetSetBoostSO) > 0)
                {
                    _listBoosts[i] = inventoryBoost;

                    _listInventoryBoosts[i].BoostImage.sprite = inventoryBoost.BoostImage.sprite;
                    _listInventoryBoosts[i].WarningImage.gameObject.SetActive(false);
                    if (_listBoosts[i].TryGetComponent(out InventoryUIBoosts inventoryUIBoosts))
                    {
                        _textDescription.text = inventoryUIBoosts.TextDesctiption.text;
                        _textDescription.gameObject.SetActive(true);
                    }
                    break;
                }
				else
				{
                    if (_listBoosts[i] != null)
                    {
                        if (_listBoosts[i].TryGetComponent(out InventoryUIBoosts inventoryUIBoosts))
                        {
                            Debug.Log(_listBoosts[i].name + " != null");
                            _textDescription.text = inventoryUIBoosts.TextDesctiption.text;
                            _textDescription.gameObject.SetActive(true);
                        }
                    }
                }
            }
        }

        public void ResetListSlot(BoostUICount inventoryBoost)
        {
            Debug.Log("ResetListSlot");
            for (int i = 0; i < _listInventoryBoosts.Length; i++)
            {
                if (_listInventoryBoosts[i] == inventoryBoost && _listBoosts[i] != null)
                {
                    _listBoosts[i] = null;

                    BoostsManager.ReturnFromInventory(_listInventoryBoosts[i].GetSetBoostSO, _listBoosts);

                    BoostsManager.Instance.MainSaveListBoosts._listBoosts[i] = null;
                    BoostsManager.Instance.SaveListBoosts._listBoosts[i] = null;

                    _listInventoryBoosts[i].BoostImage.sprite = _listInventoryBoosts[i].DefaultSprite;
                    _listInventoryBoosts[i].WarningImage.gameObject.SetActive(true);

                    OnSaveListBoosts?.Invoke();
                    break;
                }
            }
        }



        public void LoadInventory()
        {
            var boostsList = BoostsManager.Instance.MainSaveListBoosts._listBoosts;

            for (int i = 0; i < _boostsActive.Length; i++)
            {
                for (int j = 0; j < _lenListBoosts; j++)
                {
                    if (_boostsActive[i].GetSetBoostSO.name == boostsList[j])
                    {

                        LoadSlotValues(_boostsActive[i]);


                        // Debug.Log(Instance.ListBoosts[j].GetSetBoostSO.ToString());
                    }
                }
            }
        }
    }
}