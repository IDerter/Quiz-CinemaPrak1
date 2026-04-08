using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

namespace QuizCinema
{
    [Serializable]
    public struct UIElements
    {
        [SerializeField] private RectTransform[] _answersHolder;
        public RectTransform[] AnswersHolder => _answersHolder;
        [SerializeField] private TextMeshProUGUI[] _questionInfoTextObject;
        public TextMeshProUGUI[] QuestionInfoTextObject => _questionInfoTextObject;

        [SerializeField] private TextMeshProUGUI[] _countCorrectAnswer;
        public TextMeshProUGUI[] CountCorrectAnswer { get { return _countCorrectAnswer; } set { _countCorrectAnswer = value; } }

        [SerializeField] private TextMeshProUGUI _numberCurrentQuestion;
        public TextMeshProUGUI NumberCurrentQuestion { get { return _numberCurrentQuestion; } set { _numberCurrentQuestion = value; } }

        [SerializeField] private Image[] _cadrCinema;
        public Image[] CadrCinema { get { return _cadrCinema; } set { _cadrCinema = value; } }

        [Space]
        [SerializeField] private Animator _resolutionScreenAnimator;
        public Animator ResolutionScreenAnimator => _resolutionScreenAnimator;


        [Space]
        [Header("Score")]
        [SerializeField] private TextMeshProUGUI[] _scoreFinalLvl;
        public TextMeshProUGUI[] ScoreFinalLvl => _scoreFinalLvl;
        [SerializeField] private TextMeshProUGUI[] _maxScoreFinalLvl;
        public TextMeshProUGUI[] MaxScoreFinalLvl => _maxScoreFinalLvl;

        [SerializeField] private TextMeshProUGUI _countCurrentScore;
        public TextMeshProUGUI CountCurrentScore => _countCurrentScore;

        [SerializeField] private RectTransform _finishUIElements;
        public RectTransform FinishUIElements => _finishUIElements;

        [SerializeField] private GameObject _buttonFinishNextLvl;
        public GameObject EnableButtonFinishNextLvl => _buttonFinishNextLvl;

        [SerializeField] private GameObject _buttonFinishReloadLvl;
        public GameObject EnableButtonFinishReloadLvl => _buttonFinishReloadLvl;
    }
}