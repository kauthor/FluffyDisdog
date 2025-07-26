using System;
using System.Collections.Generic;
using UnityEngine;

namespace FluffyDisdog.Data
{
    [Serializable]
    public class ToolCardOpData
    {
        [SerializeField] private int id;
        [SerializeField] private string desc;
        [SerializeField] private int[] values;

        public int Id => id;
        public string Desc => desc;
        public int[] Values => values;

        public ToolCardOpData(int id, string desc, int[] values)
        {
            this.id = id;
            this.desc = desc;
            this.values = values;
        }
    }
    
    
    [CreateAssetMenu]
    public class ToolCardOptionTable:ScriptableObject
    {
        [SerializeField] private ToolCardOpData[] rawDatas;

        public void SetData(ToolCardOpData[] datas)
        => rawDatas = datas;
        
        public Dictionary<int, ToolCardOpData> TryCache()
        {
            var ret = new Dictionary<int, ToolCardOpData>();
            foreach (var d in rawDatas)
            {
                ret.Add(d.Id, d);
            }

            return ret;
        }
    }
}