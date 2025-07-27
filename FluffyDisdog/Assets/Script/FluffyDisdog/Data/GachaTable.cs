using System;
using UnityEngine;

namespace FluffyDisdog.Data
{
    [Serializable]
    public class GachaData
    {
        public int[] gachaResults;
        public int gachaRates;
    }
    
    [CreateAssetMenu]
    public class GachaTable:ScriptableObject
    {
        [SerializeField] private GachaData[] datas;
    }
}