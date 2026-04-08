using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace TowerDefense
{
    [RequireComponent(typeof(MapLevel))]
    public class BranchLevel : MonoBehaviour
    {
        [SerializeField] private MapLevel _rootLevel;
        [SerializeField] private TextMeshProUGUI _pointText;

        [SerializeField] private int _needPoints = 1;

        public void TryActivate()
        {
            gameObject.SetActive(_rootLevel.IsComplete);
            if (_needPoints < MapCompletion.Instance.TotalStars)
            {
                print(MapCompletion.Instance.TotalStars);
                _pointText.text = _needPoints.ToString();
            }
            else
            {
                _pointText.transform.parent.gameObject.SetActive(false);
                GetComponent<MapLevel>().Initialize();
            }
        }
    }
}