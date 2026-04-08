using System;
using TowerDefense;
using UnityEngine;

namespace QuizCinema
{

	public class BoostsManager : SingletonBase<BoostsManager>
    {
        public static event Action OnPressButtonBoost;
        public event Action OnAddInInventoryBoost;
        public event Action OnBuyBoost;

        public const string fileName = "boost.dat";
        [SerializeField] private int _moneyForBoosts;
        public int GetMoneyForBoosts => _moneyForBoosts;

        [Serializable]
        private class BoostSave
        {
            public string boostCorrectAnswerName;
            public int countBoost = 0;
            public int costAllBost = 0;
            public int countInInventory = 0;
        }
        [SerializeField] private BoostSave[] _save;
        [SerializeField] private BoostSave[] _mainSave;

        [Serializable]
        public class ListBoostsSave
        {
            public string[] _listBoosts = new string[3];
        }
        [SerializeField] private ListBoostsSave _saveBoosts;
        public ListBoostsSave SaveListBoosts { get { return _saveBoosts; } set { _saveBoosts = value; } }
        [SerializeField] private ListBoostsSave _mainSaveBoosts;

        public ListBoostsSave MainSaveListBoosts { get { return _mainSaveBoosts; } set { _mainSaveBoosts = value; } }

        private const string _fileNameListBoost = "boostList.dat";
        public string FileNameList => _fileNameListBoost;


        private new void Awake()
        {
            base.Awake();
            LoadData();
        }

        public virtual void LoadData()
        {
            bool flagSave = Saver<BoostSave[]>.TryLoad(fileName, ref _save);
            bool flagListSave = Saver<ListBoostsSave>.TryLoad(_fileNameListBoost, ref _saveBoosts);
            _moneyForBoosts = 0;

            if (flagSave)
            {
                for (int i = 0; i < _save.Length; i++)
                {
                    _mainSave[i] = _save[i];
                    _moneyForBoosts += _mainSave[i].costAllBost;
                }
            }

            if (flagListSave)
            {
                for (int i = 0; i < _saveBoosts._listBoosts.Length; i++)
                {
                    _mainSaveBoosts._listBoosts[i] = _saveBoosts._listBoosts[i];
                }
            }

        }

        private void Start()
        {
            if (BoostInventory.Instance != null)
                BoostInventory.Instance.OnSaveListBoosts += OnSaveListBoosts;
            if (BoostInventoryPreBooster.Instance != null)
                BoostInventoryPreBooster.Instance.OnSaveListBoosts += OnSaveListBoosts;
        }

        private void OnDestroy()
        {
            if (BoostInventory.Instance != null)
                BoostInventory.Instance.OnSaveListBoosts -= OnSaveListBoosts;

            if (BoostInventoryPreBooster.Instance != null)
                BoostInventoryPreBooster.Instance.OnSaveListBoosts -= OnSaveListBoosts;
        }

        public void ResetBoostSave()
        {
            ResetBoostValues(ref Instance._mainSave);

            ResetBoostValues(ref Instance._save);
        }

        private void ResetBoostValues(ref BoostSave[] boostList)
        {
            for (int i = 0; i < boostList.Length; i++)
            {
                boostList[i].costAllBost = 0;
                boostList[i].countBoost = 0;
                boostList[i].countInInventory = 0;

                Saver<BoostSave[]>.Save(fileName, boostList);
            }
        }

        public void ResetListSaveBoosts()
        {
            for (int i = 0; i < Instance._saveBoosts._listBoosts.Length; i++)
            {
                Instance._saveBoosts._listBoosts[i] = "";
                Saver<ListBoostsSave>.Save(_fileNameListBoost, Instance._saveBoosts);
            }

            for (int i = 0; i < Instance._mainSaveBoosts._listBoosts.Length; i++)
            {
                Instance._mainSaveBoosts._listBoosts[i] = "";
                Saver<ListBoostsSave>.Save(_fileNameListBoost, Instance._mainSaveBoosts);
            }
        }

        private void OnSaveListBoosts()
        {
            Saver<ListBoostsSave>.Save(_fileNameListBoost, Instance._mainSaveBoosts);

            Saver<BoostSave[]>.Save(fileName, Instance._mainSave);

            OnPressButtonBoost?.Invoke();
        }



        public static void BuyBoost(BoostSO asset, int numberOfBoosts)
        {
            foreach (var boost in Instance._mainSave)
            {
                if (asset.name.ToString() == boost.boostCorrectAnswerName)
                {
                    boost.countBoost += numberOfBoosts;
                    Instance._moneyForBoosts -= boost.costAllBost;
                    boost.costAllBost +=  asset.DictionaryNumberOfBoosts[numberOfBoosts];
                    Instance._moneyForBoosts += boost.costAllBost;

                    MapCompletion.Instance.MoneyShop = Instance._moneyForBoosts;

                    Instance.OnBuyBoost?.Invoke();
                    Instance.OnSaveListBoosts();
                }
            }



            /*  foreach (var boost in Instance._save)
              {
                  if (asset.name.ToString() == boost.boostCorrectAnswerName)
                  {
                      boost.countBoost += 1;
                      boost.costAllBost = boost.countBoost * asset.cost;

                      Saver<BoostSave[]>.Save(fileName, Instance._save);

                      Instance.OnSaveListBoosts();
                  }
              }
            */
        }

        public static void TakeFromInventory(BoostSO asset, BoostUICount[] listBoosts)
        {
            foreach (var boost in Instance._mainSave)
            {
                if (asset.name.ToString() == boost.boostCorrectAnswerName)
                {
                    Instance.OnAddInInventoryBoost?.Invoke();
                    //boost.countBoost -= 1;
                    //Instance._saveBoosts._listBoosts = listBoosts;
                    boost.countInInventory += 1;
                   // Saver<ListBoostsSave>.Save(_fileNameListBoost, Instance._saveBoosts);

                    Instance.OnSaveListBoosts();
                }
            }
        }

        public static void ReturnFromInventory(BoostSO asset, BoostUICount[] listBoosts)
        {
            foreach (var boost in Instance._mainSave)
            {
                if (asset.name.ToString() == boost.boostCorrectAnswerName)
                {
                    // boost.countBoost += 1;
                    boost.countInInventory -= 1;
                    //Instance._saveBoosts._listBoosts = listBoosts;

                    //Saver<ListBoostsSave>.Save(_fileNameListBoost, Instance._saveBoosts);

                    Instance.OnSaveListBoosts();
                }
            }
        }

        public static void UseBoost(BoostSO asset)
        {
            foreach (var boost in Instance._mainSave)
            {
                if (asset.name.ToString() == boost.boostCorrectAnswerName && GetCountBoost(asset) > 0)
                {
                    boost.countBoost -= 1;
                    boost.countInInventory -= 1;

                    //boost.costAllBost = boost.countBoost * asset.cost;
                    DeleteSaveInventoryBoost(asset);

                    Saver<BoostSave[]>.Save(fileName, Instance._mainSave);
                    Saver<BoostSave[]>.Save(fileName, Instance._save);

                    OnPressButtonBoost?.Invoke();
                    break;
                }
            }
        }

        public static void DeleteSaveInventoryBoost(BoostSO asset)
        {
            var listBoost = Instance._mainSaveBoosts._listBoosts;
            for (int i = 0; i < listBoost.Length; i++)
            {
                if (listBoost[i] == asset.name)
                {
                    Instance._mainSaveBoosts._listBoosts[i] = null;
                    Instance._saveBoosts._listBoosts[i] = null;
                    Instance.OnSaveListBoosts();
                    break;
                }
            }
        }

        public static void SetBoostLearningInInventory(BoostSO asset)
		{
            foreach (var boost in Instance._mainSave)
            {
                if (asset.name.ToString() == boost.boostCorrectAnswerName)
                {
                    boost.countBoost = 1;

                    Instance.OnSaveListBoosts();
                }
            }
        }

        public static int GetCostBoost(BoostSO asset)
        {
            foreach (var boost in Instance._mainSave)
            {
                if (asset.name.ToString() == boost.boostCorrectAnswerName)
                {
                    return boost.costAllBost;
                }
            }
            return 0;
        }

        public static int GetCountBoost(BoostSO asset)
        {
            foreach (var boost in Instance._mainSave)
            {
                if (boost.boostCorrectAnswerName == asset.name.ToString())
                {
                    return boost.countBoost;
                }
            }
            return 0;
        }

        public static int GetCountInInventoryBoost(BoostSO asset)
        {
            foreach (var boost in Instance._mainSave)
            {
                if (boost.boostCorrectAnswerName == asset.name.ToString())
                {
                    return boost.countInInventory;
                }
            }
            return 0;
        }
        /*
        public static int GetUpgradeLevel(UpgradeAsset asset)
        {
            foreach (var upgrade in Instance.save)
            {
                if (upgrade.asset == asset)
                {
                    Debug.Log(asset.name);
                    return upgrade.level;
                }
            }
            Debug.Log("!!!!");
            return 0;
        }

       
        */
    }
}