using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TowerDefense;
using UnityEngine;
using static QuizCinema.InterfaceBoost;

namespace QuizCinema
{
    public class BoostInventory : SingletonBase<BoostInventory>, IBoostInventory
    {
        public event Action OnSaveListBoosts;
        public event Action<BoostUICount> OnPressButtonBoostInventory;

        [SerializeField] private BoostUICount[] _listBoosts = new BoostUICount[3];
        public BoostUICount[] ListBoosts {get { return _listBoosts; } set { _listBoosts = value; } }

        [SerializeField] private BoostSlot[] _listInventoryBoosts;
        public BoostSlot[] ListInventoryBoosts => _listInventoryBoosts;

        [SerializeField] private BoostUICount[] _boostsActive;
        public InventoryUIBoosts[] BoostActive => (InventoryUIBoosts[])_boostsActive;

        [SerializeField] private TextMeshProUGUI _textDescription;
        [SerializeField] private TextMeshProUGUI _textDescriptionDefault;

        private const string _clickSFX = "ClickSFX";

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

        }

        public void FillList(BoostUICount inventoryBoost)
        {
            for (int i = 0; i < _listBoosts.Length; i++)
            {
                if (_listBoosts[i] == null && BoostsManager.GetCountBoost(inventoryBoost.GetSetBoostSO) > 0)
                {
                    _listBoosts[i] = inventoryBoost;

                    AudioManager.Instance.PlaySound(_clickSFX);

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

                    if (_listBoosts[i].TryGetComponent(out InventoryUIBoosts inventoryUIBoosts) && _textDescription != null)
                    {
                        _textDescription.text = inventoryUIBoosts.TextDesctiption.text;
                    }
                    break;
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

                    AudioManager.Instance.PlaySound(_clickSFX);

                    BoostsManager.ReturnFromInventory(_listInventoryBoosts[i].GetSetBoostSO, _listBoosts);

                    BoostsManager.Instance.MainSaveListBoosts._listBoosts[i] = null;
                    BoostsManager.Instance.SaveListBoosts._listBoosts[i] = null;

                    _listInventoryBoosts[i].BoostImage.sprite = _listInventoryBoosts[i].DefaultSprite;
                    _listInventoryBoosts[i].WarningImage.gameObject.SetActive(true);

                    if (_listBoosts[0] == null && _listBoosts[1] == null && _listBoosts[2] == null)
                    {
                        _textDescription.text = _textDescriptionDefault.text;
                        Debug.Log("ALL ZERO VALUES!");
                    }


                    OnSaveListBoosts?.Invoke();
                    break;
                }
            }
        }

        public void LoadInventory()
        {
            var savedBoosts = BoostsManager.Instance.MainSaveListBoosts._listBoosts;

            for (int i = 0; i < savedBoosts.Length; i++)
            {
                if (string.IsNullOrEmpty(savedBoosts[i]))
                {
                    continue;
                }

                for (int j = 0; j < _boostsActive.Length; j++)
                {
                    if (_boostsActive[j].GetSetBoostSO.name == savedBoosts[i])
                    {
                        _listBoosts[i] = _boostsActive[j];
                        _listInventoryBoosts[i].GetSetBoostSO = _boostsActive[j].GetSetBoostSO;
                        _listInventoryBoosts[i].BoostImage.sprite = _boostsActive[j].BoostImage.sprite;
                        _listInventoryBoosts[i].WarningImage.gameObject.SetActive(false);

                        if (_listBoosts[i].TryGetComponent(out InventoryUIBoosts inventoryUIBoosts) && _textDescription != null)
                        {
                            _textDescription.text = inventoryUIBoosts.TextDesctiption.text;
                        }
                        break;
                    }
                }
            }
        }
    }
}