using System;
using System.Collections.Generic;
using System.Linq;
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
        
        public List<BoxItemData> TryCache()
        {
            return data.ToList();
        }
    }
}