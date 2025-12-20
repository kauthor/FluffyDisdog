using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FluffyDisdog;
using FluffyDisdog.Data;
using FluffyDisdog.Data.RelicData;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Script.FluffyDisdog.Managers
{
    public class ExcelManager:CustomSingleton<ExcelManager>
    {
        private ToolTable _toolTable;
        private RelicDataTable _relicDataTable;
        private ToolExcelDataTable _toolExcelDatas;
        private ToolCardOptionTable _toolCardOptionTable;
        private GachaTable _gachaTable;
        
        private TagTable _tagTable;

        private Dictionary<ToolType, ToolData> toolDataDic;
        private Dictionary<RelicName, RelicData> relicDataDic;
        private Dictionary<ToolType, ToolExcelData> toolExcelDataDic;
        private Dictionary<int, ToolCardOpData> toolCardOpDataDic;
        private Dictionary<int, GacheInGameData> gacheInGameDataDic;
        private Dictionary<int, TagData> tagDataDic;

        protected override void Awake()
        {
            base.Awake();
            
            LoadTable().Forget();
            
        }

        private async UniTaskVoid LoadTable()
        {
            AsyncOperationHandle relicHandle =
                Addressables.LoadAssetAsync<RelicDataTable>("RelicTable");
            relicHandle.Completed += op =>
            {
                var res = op.Result as RelicDataTable;
                _relicDataTable = res;
            };
            await relicHandle;
            Addressables.Release(relicHandle);
            
            AsyncOperationHandle toolhandle =
                Addressables.LoadAssetAsync<ToolTable>("ToolTable");
            toolhandle.Completed += op =>
            {
                var res = op.Result as ToolTable;
                _toolTable = res;
            };
            await toolhandle;
            Addressables.Release(toolhandle);
            
            AsyncOperationHandle texcelHandle =
                Addressables.LoadAssetAsync<ToolExcelDataTable>("ToolExcelTable");
            texcelHandle.Completed += op =>
            {
                var res = op.Result as ToolExcelDataTable;
                _toolExcelDatas = res;
            };
            await texcelHandle;
            Addressables.Release(texcelHandle);
            
            AsyncOperationHandle topHandle =
                Addressables.LoadAssetAsync<ToolCardOptionTable>("ToolCardOptionTable");
            topHandle.Completed += op =>
            {
                var res = op.Result as ToolCardOptionTable;
                _toolCardOptionTable = res;
            };
            await topHandle;
            Addressables.Release(topHandle);
            
            AsyncOperationHandle gachaHandle =
                Addressables.LoadAssetAsync<GachaTable>("GachaTable");
            gachaHandle.Completed += op =>
            {
                var res = op.Result as GachaTable;
                _gachaTable = res;
            };
            await gachaHandle;
            Addressables.Release(gachaHandle);
            
            AsyncOperationHandle tagHandle =
                Addressables.LoadAssetAsync<TagTable>("TagTable");
            tagHandle.Completed += op =>
            {
                var res = op.Result as TagTable;
                _tagTable = res;
            };
            await tagHandle;
            Addressables.Release(tagHandle);
            
            toolDataDic = _toolTable.TryCache();
            relicDataDic = _relicDataTable.TryCache();
            toolExcelDataDic = _toolExcelDatas.TryCache();
            toolCardOpDataDic = _toolCardOptionTable.TryCache();
            gacheInGameDataDic = _gachaTable.TryCache();
            tagDataDic = _tagTable.TryCache();
        }

        public ToolData GetToolData(ToolType t)
        {
            return toolDataDic[t];
        }

        public RelicData GetRelicData(RelicName relicName)
        {
            return relicDataDic[relicName];
        }

        public ToolExcelData GetToolExcelData(ToolType t)
        {
            return toolExcelDataDic[t];
        }

        public ToolCardOpData GetToolCardOpData(ToolType t)
        {
            var data = GetToolExcelData(t);
            if(data.AddedOptionIds[0] > 99)
               return toolCardOpDataDic[data.AddedOptionIds[0]];
            return null;
        }

        public TagData GetTagData(int id)
        {
            return tagDataDic[id];
        }
    }
}