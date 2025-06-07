using System.Collections.Generic;
using FluffyDisdog;
using FluffyDisdog.Data;
using FluffyDisdog.Data.RelicData;
using UnityEngine;

namespace Script.FluffyDisdog.Managers
{
    public class ExcelManager:CustomSingleton<ExcelManager>
    {
        //어드레서블 로드로 바꾸자...
        [SerializeField] private ToolTable _toolTable;
        [SerializeField] private RelicDataTable _relicDataTable;

        private Dictionary<ToolType, ToolData> toolDataDic;
        private Dictionary<RelicName, RelicData> relicDataDic;

        protected override void Awake()
        {
            base.Awake();
            toolDataDic = _toolTable.TryCache();
            relicDataDic = _relicDataTable.TryCache();
        }

        public ToolData GetToolData(ToolType t)
        {
            return toolDataDic[t];
        }

        public RelicData GetRelicData(RelicName relicName)
        {
            return relicDataDic[relicName];
        }
    }
}