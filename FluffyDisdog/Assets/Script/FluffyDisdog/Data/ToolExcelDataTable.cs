using System;
using System.Collections.Generic;
using UnityEngine;

namespace FluffyDisdog.Data
{
    [Serializable]
    public class ToolExcelData
    {
        [SerializeField] ToolType toolType;
        [SerializeField] ToolTag toolTag;
        [SerializeField] ToolAdditionalOption option;
        [SerializeField] int optionValue;
        
        public ToolType ToolType => toolType;
        public ToolTag ToolTag => toolTag;
        public ToolAdditionalOption Option => option;
        public int OptionValue => optionValue;
        
        public ToolExcelData(ToolType type, ToolTag tag, ToolAdditionalOption op, int val)
        {
            this.toolType = type;
            this.toolTag = tag;
            this.option = op;
            this.optionValue = val;
        }
    }
    
    
    [CreateAssetMenu]
    public class ToolExcelDataTable : ScriptableObject
    {
        [SerializeField] private ToolExcelData[] _toolExcelDatas;
        
        public void SetToolExcelData(ToolExcelData[] arr) => _toolExcelDatas = arr;
        
        public Dictionary<ToolType, ToolExcelData> TryCache()
        {
            var ret = new Dictionary<ToolType, ToolExcelData>();
            for (int i = 0; i < _toolExcelDatas.Length; i++)
            {
                ret.Add(_toolExcelDatas[i].ToolType, _toolExcelDatas[i]);
            }

            return ret;
        }
    }
}