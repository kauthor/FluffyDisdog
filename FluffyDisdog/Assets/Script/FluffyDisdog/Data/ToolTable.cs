using System;
using System.Collections.Generic;
using UnityEngine;

namespace FluffyDisdog.Data
{
    [Serializable]
    public class ToolData
    {
        public ToolType type;
        public int cellWidth;
        public int cellHeight;
        public int[] ratio;
        public int Center;

        public int[] GetRatioValues()
        {
            int[] newArr = new int[ratio.Length];
            for (int i = 0; i < ratio.Length; i++)
            {
                newArr[i] = ratio[i];
            }

            return newArr;
        }
        
        public ToolData Copy()
        {
            var newArr = GetRatioValues();
            return new ToolData()
            {
                type = this.type,
                cellHeight = this.cellHeight,
                cellWidth = this.cellWidth,
                ratio = newArr,
                Center = this.Center
            };
        }
    }
    
    [CreateAssetMenu]
    public class ToolTable:ScriptableObject
    {
        [SerializeField] private ToolData[] _datas;

#if UNITY_EDITOR
        public void SetData(ToolData[] d)
        {
            _datas = d;
        }
#endif

        public Dictionary<ToolType, ToolData> TryCache()
        {
            var ret = new Dictionary<ToolType, ToolData>();

            foreach (var d in _datas)
            {
                ret.Add(d.type, d.Copy());
            }

            return ret;
        }
    }
}