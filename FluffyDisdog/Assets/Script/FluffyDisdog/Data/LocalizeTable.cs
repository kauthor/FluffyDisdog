using System;
using System.Collections.Generic;
using UnityEngine;

namespace FluffyDisdog.Data
{
    [Serializable]
    public class LocalizeData
    {
        public string key;
        public string kor;
        public string eng;
    }
    
    
    [CreateAssetMenu]
    public class LocalizeTable:ScriptableObject
    {
        [SerializeField] private LocalizeData[] localizeDatas;
        
#if UNITY_EDITOR
        public void SetData(LocalizeData[] d)
        {
            localizeDatas = d;
        }
#endif

        public Dictionary<string, LocalizeData> TryCache()
        {
            var ret = new Dictionary<string, LocalizeData>();

            foreach (var d in localizeDatas)
            {
                ret.Add(d.key, d);
            }
            
            return ret;
        }
    }
}