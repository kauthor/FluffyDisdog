using System;
using System.Collections.Generic;
using UnityEngine;

namespace FluffyDisdog.Data
{
    [Serializable]
    public class MapData
    {
        public int id;
        public int mapKey;
        public string mapName;
        public int shopRerollCost;
        public int stageCardRewardGachaId;
    }
    [CreateAssetMenu]
    public class MapTable:ScriptableObject
    {
        [SerializeField] private MapData[] data;

#if UNITY_EDITOR
        public void SetData(MapData[] d)
        {
            data = d;
        }
#endif
        
        public Dictionary<int, MapData> TryCache()
        {
            Dictionary<int, MapData> ret = new Dictionary<int, MapData>();
            foreach (var d in data)
                ret.Add(d.mapKey,d);
            
            return ret;
        }
    }
}