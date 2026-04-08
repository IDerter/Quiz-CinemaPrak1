using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuizCinema
{
    public class ShopManager : MonoBehaviour
    {
        [SerializeField] private AudioClip _audioClipButtonClick;

        public void OpenPanel( GameObject gameObject)
        {
            gameObject.SetActive(true);
            AudioManager.Instance.PlaySoundAudioClip(_audioClipButtonClick);
        }

        public void ClosePanel(GameObject gameObject)
        {
            gameObject.SetActive(false);
            AudioManager.Instance.PlaySoundAudioClip(_audioClipButtonClick);
        }


    }
}