using System.Collections.Generic;
using FluffyDisdog;
using FluffyDisdog.Data;
using UnityEngine;

namespace Script.FluffyDisdog.Managers
{
    public class ExcelManager:CustomSingleton<ExcelManager>
    {
        //어드레서블 로드로 바꾸자...
        [SerializeField] private ToolTable _toolTable;

        private Dictionary<ToolType, ToolData> toolDataDic;

        protected override void Awake()
        {
            base.Awake();
            toolDataDic = _toolTable.TryCache();
        }

        public ToolData GetToolData(ToolType t)
        {
            return toolDataDic[t];
        }
    }
}