using System;
using System.Collections;
using System.Collections.Generic;
using TowerDefense;
using UnityEngine;

namespace QuizCinema
{
    public class SkinManager : SingletonBase<SkinManager>
    {
        public event Action OnBuySkin;
        public event Action<SkinSave> OnPutOn;

        [SerializeField] private int _moneyForSkins;
        public int GetMoneyForSkins => _moneyForSkins;

        [Serializable]
        public class SkinSave
        {
            [SerializeField] private string _skinName;
            public string SkinName { get { return _skinName; } set { _skinName = value; } }

            [SerializeField] private bool _buy;
            public bool Buy { get { return _buy; } set { _buy = value; } }

            [SerializeField] private bool _putOn;
            public bool PutOn { get { return _putOn; } set { _putOn = value; } }

            [SerializeField] private int _costSkin;
            public int CostSkin { get { return _costSkin; } set { _costSkin = value; } }
            [SerializeField] private string _boostName;
            public string BoostName { get { return _boostName; } set { _boostName = value; } }
        }
        [SerializeField] private SkinSave[] _save;
        [SerializeField] private SkinSave[] _mainSave;
        [SerializeField] private SkinSO _deffaultSkinSO;
        public SkinSO DeffaultSkinSO => _deffaultSkinSO;
        public SkinSave[] MainSave => _mainSave;

        private const string _fileNameSkin = "skinSave.dat";
        public string FileNameSkin => _fileNameSkin;

        private new void Awake()
        {
            base.Awake();
            LoadData();
            Debug.Log("Load Data Skin Manager");
        }

		public void LoadData()
		{
            bool flagSave = Saver<SkinSave[]>.TryLoad(_fileNameSkin, ref _save);
            _moneyForSkins = 0;

            if (flagSave)
            {
                for (int i = 0; i < _save.Length; i++)
                {
                    _mainSave[i] = _save[i];
                    _moneyForSkins += _mainSave[i].CostSkin;
                }
            }
        }


        private void ResetSkinValues(ref SkinSave[] skinSave)
        {
            _save[0].Buy = true;
            _save[0].PutOn = true;

            for (int i = 1; i < _save.Length; i++)
            {
                _save[i].Buy = false;
                _save[i].PutOn = false;
                _save[i].CostSkin = 0;

                Saver<SkinSave[]>.Save(_fileNameSkin, skinSave);
            }
        }

        public void ResetSkinSave()
		{
            ResetSkinValues(ref Instance._save);

            ResetSkinValues(ref Instance._mainSave);
        }

        private void OnSaveSkins()
        {
            Saver<SkinSave[]>.Save(_fileNameSkin, Instance._mainSave);
            Debug.Log("Save Skins");
        }

        public static void BuySkin(SkinSO asset)
        {
            foreach (var skin in Instance._mainSave)
            {
                if (asset.name.ToString() == skin.SkinName)
                {
                    skin.Buy = true;
                    skin.SkinName = asset.name;
                    if (!asset.IsPurshase)
                    {
                        Instance._moneyForSkins += asset.cost;
                        skin.CostSkin = asset.cost;
                    }

                    skin.BoostName = asset.GetBoostName;

                    MapCompletion.Instance.SkinShop = Instance._moneyForSkins;
                    SkinLoader.Instance.UpdateSkinSave(skin);

                    Instance.OnSaveSkins();
                    Instance.OnBuySkin?.Invoke();

                    AnalyticsManager.Instance.SaveNameSkin(skin.SkinName);
                }
            }
        }

        public static void PutOnSkin(SkinSO asset)
		{
            foreach (var skin in Instance._mainSave)
            {
                Debug.Log(asset.name + " " + skin.SkinName);
                if (asset.name.ToString() == skin.SkinName)
                {
                    skin.PutOn = true;
                    SkinLoader.Instance.UpdateSkinSave(skin);
                    Instance.OnPutOn?.Invoke(skin);
                }
				else
				{
                    skin.PutOn = false;
                    SkinLoader.Instance.UpdateSkinSave(skin);
                }
            }
            Instance.OnSaveSkins();
        }

        public static void TakeOffSkin(SkinSO asset)
        {
            foreach (var skin in Instance._mainSave)
            {
                if (asset.name.ToString() == skin.SkinName)
                {
                    skin.PutOn = false;
                    SkinLoader.Instance.UpdateSkinSave(skin);
                }
            }
            
            if (CheckNoEquippedSkin() == true)
			{
                PutOnSkin(Instance?._deffaultSkinSO);
			}
            Instance.OnSaveSkins();
        }

        public static SkinSave GetPutOnSkin()
		{
            foreach (var skin in Instance._mainSave)
			{
                if (skin.PutOn)
				{
                    return skin;
				}
			}
            return null;
        }

        public static int GetIndexPutOnSkin()
        {
            for (int i = 0; i < Instance._mainSave.Length; i++)
			{
                if (Instance._mainSave[i].PutOn)
                    return i;
			}
            return 0;
        }

        public static bool CheckNoEquippedSkin()
		{
            foreach (var skin in Instance._mainSave)
			{
                if (skin.PutOn)
                    return false;
			}
            return true;
		}

        public static bool CheckIsBuySkin(SkinSO skinSO)
		{
            foreach (var skin in Instance._mainSave)
			{
                if (skin.SkinName == skinSO.name)
				{
                    return skin.Buy;
				}
			}
            return false;
        }
    }
}