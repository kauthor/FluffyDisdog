using System;
using System.Collections.Generic;
using UnityEngine;

namespace FluffyDisdog.Data
{
    [Serializable]
    public class ShopData
    {
        public int id;
        public int mapKey;
        public int slotIndex;
        public int gachaId;
    }
    [CreateAssetMenu]
    public class ShopTable:ScriptableObject
    {
        [SerializeField] private ShopData[] data;

#if UNITY_EDITOR
        public void SetData(ShopData[] d)
        {
            data = d;
        }
#endif
        
        public Dictionary<int, ShopData> TryCache()
        {
            Dictionary<int, ShopData> ret = new Dictionary<int, ShopData>();
            foreach (var d in data)
                ret.Add(d.id,d);
            
            return ret;
        }
    }
}