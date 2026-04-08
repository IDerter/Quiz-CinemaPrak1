using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace QuizCinema
{
    public class SceneDependenciesContainer : Dependency
    {
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private Score _score;
        [SerializeField] private LvlData _lvl;
        [SerializeField] private QuestionMethods _questionMethods;
        [SerializeField] private Timer _timer;
        [SerializeField] private BoostIncreaseOn50PersentScore _boostIncrease50Percent;

        protected override void BindAll(MonoBehaviour monoBehaviourInScene)
        {
            Bind<GameManager>(_gameManager, monoBehaviourInScene);
            Bind<Score>(_score, monoBehaviourInScene);
            Bind<LvlData>(_lvl, monoBehaviourInScene);
            Bind<QuestionMethods>(_questionMethods, monoBehaviourInScene);
            Bind<Timer>(_timer, monoBehaviourInScene);
            Bind<BoostIncreaseOn50PersentScore>(_boostIncrease50Percent, monoBehaviourInScene);
        }



        private void Awake()
        {
            FindAllObjectToBind();
        }
    }
}