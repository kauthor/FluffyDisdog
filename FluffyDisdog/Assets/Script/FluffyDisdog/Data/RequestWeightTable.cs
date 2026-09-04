using System;
using System.Collections.Generic;
using UnityEngine;

namespace FluffyDisdog.Data
{
    [Serializable]
    public class RequestWeightData
    {
        public int id;
        public int groupId;
        public string cardKey;
        public int gachaId;
        public int rate;
    }
    public class RequestWeightTable:ScriptableObject
    {
        [SerializeField] private RequestWeightData[] weightData;
#if UNITY_EDITOR
        public void SetData(RequestWeightData[] d)
        {
            weightData = d;
        }
#endif
        
        public Dictionary<int, Dictionary<string,RequestWeightData>> TryCache()
        {
            var ret = new Dictionary<int, Dictionary<string,RequestWeightData>>();
            foreach (var d in weightData)
            {
                if(!ret.ContainsKey(d.groupId))
                    ret[d.groupId] = new Dictionary<string, RequestWeightData>();
                ret[d.groupId].Add(d.cardKey, d);
            }
            return ret;
        }
    }
}