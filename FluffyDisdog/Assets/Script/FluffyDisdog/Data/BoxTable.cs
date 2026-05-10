using System;
using System.Collections.Generic;
using UnityEngine;

namespace FluffyDisdog.Data
{
    [Serializable]
    public class BoxData
    {
        public int id;
        public int boxId;
        public int pickCount;
    }
    public class BoxTable:ScriptableObject
    {
        [SerializeField] private BoxData[] data;

#if UNITY_EDITOR
        public void SetData(BoxData[] d)
        {
            data = d;
        }
#endif
        
        public Dictionary<int, BoxData> TryCache()
        {
            Dictionary<int, BoxData> ret = new Dictionary<int, BoxData>();
            foreach (var d in data)
                ret.Add(d.boxId,d);
            
            return ret;
        }
    }
}