using System;
using System.Collections.Generic;
using UnityEngine;

namespace FluffyDisdog.Data
{
    [Serializable]
    public class BoxItemData
    {
        public int id;
        public int boxId;
        public int rewardType;
        public int rewardValue;
        public int rewardCount;
    }
    [CreateAssetMenu]
    public class BoxItemTable:ScriptableObject
    {
        [SerializeField] private BoxItemData[] data;
#if UNITY_EDITOR
        public void SetData(BoxItemData[] d)
        {
            data = d;
        }
#endif
        
        public Dictionary<int, BoxItemData> TryCache()
        {
            Dictionary<int, BoxItemData> ret = new Dictionary<int, BoxItemData>();
            foreach (var d in data)
                ret.Add(d.boxId,d);
            
            return ret;
        }
    }
}