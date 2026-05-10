using System;
using System.Collections.Generic;
using UnityEngine;

namespace FluffyDisdog.Data
{
    [Serializable]
    public class PackData
    {
        public string id;
        public string nameId;
        public int gachaKey;
        public int cardShow;
        public int cardPick;
        public string packResource;
    }
    [CreateAssetMenu]
    public class PackTable:ScriptableObject
    {
        [SerializeField] private PackData[] data;

#if UNITY_EDITOR
        public void SetData(PackData[] d)
        {
            data = d;
        }
#endif
        
        public Dictionary<string, PackData> TryCache()
        {
            Dictionary<string, PackData> ret = new Dictionary<string, PackData>();
            foreach (var d in data)
                ret.Add(d.id,d);
            
            return ret;
        }
    }
}