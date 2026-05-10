using System;
using System.Collections.Generic;
using UnityEngine;

namespace FluffyDisdog.Data
{
    [Serializable]
    public class ShopItemData
    {
        public int id;
        public int itemType;
        public string itemId;
        public int costMin;
        public int costMax;
    }
    [CreateAssetMenu]
    public class ShopItemTable:ScriptableObject
    {
        [SerializeField] private ShopItemData[] data;

#if UNITY_EDITOR
        public void SetData(ShopItemData[] d)
        {
            data = d;
        }
#endif
        
        public Dictionary<int, ShopItemData> TryCache()
        {
            Dictionary<int, ShopItemData> ret = new Dictionary<int, ShopItemData>();
            foreach (var d in data)
                ret.Add(d.id,d);
            
            return ret;
        }
    }
}