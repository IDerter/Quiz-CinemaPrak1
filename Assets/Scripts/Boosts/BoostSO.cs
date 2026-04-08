using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuizCinema
{
    [CreateAssetMenu]
    [System.Serializable()]
    public sealed class BoostSO : BuySO 
    {
        [SerializedDictionary("CountBoostsInOffer", "CostByOffer")]
        public SerializedDictionary<int, int> DictionaryNumberOfBoosts;
        [SerializedDictionary] public Dictionary<int, int> costBuyNumberOfBoosts;

    }
}
