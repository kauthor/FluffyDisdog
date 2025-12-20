using System;
using System.Collections.Generic;
using UnityEngine;

namespace FluffyDisdog.Data
{
    [Serializable]
    public class TagData
    {
        [SerializeField] public int id;
        [SerializeField] public int bit;
        [SerializeField] public string nameKey;
        [SerializeField] public string descKey;
        [SerializeField] public string tagImage;
        [SerializeField] public int[] values;

#if UNITY_EDITOR
        public TagData(int id, int bit, string nameKey, string descKey, string tagImage, int[] values)
        {
            this.id = id;
            this.bit = bit;
            this.nameKey = nameKey;
            this.descKey = descKey;
            this.tagImage = tagImage;
            this.values = values;
        }
#endif
    }
    public class TagTable:ScriptableObject
    {
        [SerializeField] private TagData[] tagData;
        
        public TagData GetTagData(int id) => tagData[id];

        public Dictionary<int, TagData> TryCache()
        {
            Dictionary<int, TagData> ret = new Dictionary<int, TagData>();
            foreach (var tag in tagData)
            {
                ret.Add(tag.id, tag);
            }

            return ret;
        }
        
#if UNITY_EDITOR
        public void SetTagData(TagData[] tagData) => this.tagData = tagData;
#endif
    }
}