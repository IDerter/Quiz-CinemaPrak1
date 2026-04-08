using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace QuizCinema
{
    public class TextMeshProTest : MonoBehaviour, IDependency<TextMeshProUGUI>
    {
        [SerializeField] private TextMeshProUGUI _timerText;
        public TextMeshProUGUI TimerText => _timerText;
        public void Construct(TextMeshProUGUI obj)
        {
            Debug.Log("онаедю!!!");
            _timerText = obj;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}