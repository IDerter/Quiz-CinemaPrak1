using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static QuizCinema.DataLikeCinema;

namespace QuizCinema
{
    public class SpawnLikeCinema : MonoBehaviour
    {
        [SerializeField] private LikeCinemaPanel _prefabLikeCinema;
        [SerializeField] private RectTransform _spawnParent;
        [SerializeField] private CinemaInfo _cinemaInfo;
        [SerializeField] private Scrollbar _scrollbar;

        private void OnEnable()
        {
            _spawnParent.transform.position = new Vector3(_spawnParent.transform.position.x, _spawnParent.transform.position.y, _spawnParent.transform.position.z);
            var list = DataLikeCinema.Instance.CompletionDataCinema;
            var likeCinemaPanels = _spawnParent.GetComponentsInChildren<LikeCinemaPanel>();
            foreach (var likeCinema in likeCinemaPanels)
                Destroy(likeCinema.gameObject);

            foreach (var likeCinema in list)
            {
                var spawnObj = Instantiate(_prefabLikeCinema, _spawnParent);

                Debug.Log($"Posters/{ likeCinema.CadrCinemaName}_poster");
                Sprite sprite = Resources.Load($"Posters/{likeCinema.CadrCinemaName}_poster", typeof(Sprite)) as Sprite;
                // Debug.Log(sprite.name);
                if (likeCinema.Question._answerType == AnswerType.Single)
				{
                    foreach (var answer in likeCinema.Question.Answers)
                    {
                        if (answer.IsCorrect)
                        {
                            spawnObj.GetComponent<LikeCinemaPanel>().TextCinemaName.text = answer.InfoList[PlayerPrefs.GetInt("IndexLanguageSave")];
                        }
                    }
                    spawnObj.GetComponent<LikeCinemaPanel>().PosterImage.sprite = sprite;
                    spawnObj.GetComponent<LikeCinemaPanel>().TextCinemaInfo.text = likeCinema.Question.ListDescriptionFilm[PlayerPrefs.GetInt("IndexLanguageSave")];
                    spawnObj.GetComponent<LikeCinemaPanel>().CinemaInfo = likeCinema;
                }
				else
				{
                    spawnObj.GetComponent<LikeCinemaPanel>().PosterImage.sprite = sprite;

                    var cinemaNameEng = likeCinema.Question._cadrCinemaName.Split("!");
                    var cinemaNameRu = likeCinema.Question._cadrCinemaNameTranslateRu.Split("!");
                    var directorCinemaEng = likeCinema.Question.ListDescriptionFilm[0].Split("!");
                    var directorCinemaRu = likeCinema.Question.ListDescriptionFilm[1].Split("!");

                    Debug.Log(cinemaNameEng[0] + cinemaNameEng[1]);
                    Debug.Log(cinemaNameRu[0] + cinemaNameRu[1]);

                    var indexFilm = 0;
                    for (int i = 0; i < cinemaNameEng.Length; i++)
					{
                        if (cinemaNameEng[i] == likeCinema.CadrCinemaName)
						{
                            indexFilm = i;
                        }
					}
					if (PlayerPrefs.GetInt("IndexLanguageSave") == 0)
					{
                        spawnObj.GetComponent<LikeCinemaPanel>().TextCinemaName.text = cinemaNameEng[indexFilm];
                        spawnObj.GetComponent<LikeCinemaPanel>().TextCinemaInfo.text = directorCinemaEng[indexFilm];
                    }
                    else if (PlayerPrefs.GetInt("IndexLanguageSave") == 1)
					{
                        spawnObj.GetComponent<LikeCinemaPanel>().TextCinemaName.text = cinemaNameRu[indexFilm];
                        spawnObj.GetComponent<LikeCinemaPanel>().TextCinemaInfo.text = directorCinemaRu[indexFilm];
                    }
                   // var cinemaName = likeCinema.Question._cadrCinemaName
				}
            }
        }
    }
}