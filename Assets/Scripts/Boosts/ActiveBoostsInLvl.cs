using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static QuizCinema.InterfaceBoost;

namespace QuizCinema
{
    public class ActiveBoostsInLvl : SingletonBase<ActiveBoostsInLvl>, IBoostInLvl
    {
        [SerializeField] private BoostUICount[] _boostsActive;
        [SerializeField] private Transform[] _sceneBoosts;

        [SerializeField] private Transform _skinTransform;
        [SerializeField] private BoostInGame[] _boostsActiveForSkins;

        [SerializeField] private AnswersMethods _answersMethods;
        [SerializeField] private BoostInventory _boostInventory;

        private const int _maxBoostInLvl = 3;
        public int GetMaxBoostInLvl => _maxBoostInLvl;

        private bool _isCreateBoost = false;

        private void OnEnable()
        {
            _answersMethods.OnCreateAnswers += OnCreateAnswers;
        }

        private void OnCreateAnswers(Question obj)
        {
            Activate();
        }

        private void OnDestroy()
        {
            _answersMethods.OnCreateAnswers -= OnCreateAnswers;
        }

        public void Activate()
        {
            if (_isCreateBoost == false)
            {
                var skinPutOn = SkinManager.GetPutOnSkin();
                Debug.Log(skinPutOn.SkinName);
                foreach (var boostSkin in _boostsActiveForSkins)
				{
                    Debug.Log(boostSkin.GetSetBoostSO.name);
                    if (boostSkin.GetSetBoostSO.name == skinPutOn.BoostName)
					{
                        boostSkin.gameObject.SetActive(true);
                        if (boostSkin.TryGetComponent(out BoostInGame boostComponent))
						{
                            Debug.Log("component - " + boostComponent);
                            if (boostComponent.IsAllLvlActive)
                            {
                                boostSkin.GetComponentInChildren<Button>().onClick.Invoke();
                                Debug.Log("Active all lvl skin!!!");
                            }
                        }
                        _skinTransform.gameObject.SetActive(true);
                    }
				}

                var boostsList = BoostsManager.Instance.MainSaveListBoosts._listBoosts;

                for (int i = 0; i < _boostsActive.Length; i++)
                {
                    for (int j = 0; j < boostsList.Length; j++)
                    {
                        if (_boostsActive[i].GetSetBoostSO.name == boostsList[j])
                        {
                            // var test = Instantiate(_boostsActive[j], _sceneBoosts[j]);
                            var boost = Instantiate(_boostsActive[i], _sceneBoosts[j]);
                            _sceneBoosts[j].gameObject.SetActive(true);

                            boost.gameObject.SetActive(true);
                            if (boost.TryGetComponent(out BoostInGame boostComponent))
                            {
                                if (boostComponent.IsAllLvlActive)
                                {
                                    boost.GetComponentInChildren<Button>().onClick.Invoke();
                                    Debug.Log("Active all lvl booster!!!");
                                }
                            }

                            if (boost.GetSetBoostSO.name == "Freeze Time")
							{
                            }
                            // _boostsActive[i].gameObject.SetActive(true);
                        }
                    }
                }
            }
            _isCreateBoost = true;
        }

        public void LoadInventory()
        {
            var boostsList = BoostsManager.Instance.MainSaveListBoosts._listBoosts;

            for (int i = 0; i < _boostsActive.Length; i++)
            {
                for (int j = 0; j < _maxBoostInLvl; j++)
                {
                    if (_boostsActive[i].GetSetBoostSO.name == boostsList[j])
                    {
                        _boostInventory.ListBoosts[j] = _boostsActive[i];
                    }
                }
            }
        }
    }
}