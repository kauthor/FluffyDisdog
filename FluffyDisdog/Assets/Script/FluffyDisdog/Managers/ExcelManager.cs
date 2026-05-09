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
        private LocalizeTable _localizeTable;

        private Dictionary<ToolType, ToolData> toolDataDic;
        private Dictionary<RelicName, RelicData> relicDataDic;
        private Dictionary<ToolType, ToolExcelData> toolExcelDataDic;
        private Dictionary<int, ToolCardOpData> toolCardOpDataDic;
        private Dictionary<int, GacheInGameData> gacheInGameDataDic;
        private Dictionary<int, TagData> tagDataDic;
        private Dictionary<string, LocalizeData> localizeDataDic;
        
        
        private bool initialized=false;
        public bool Initialized => initialized;

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
            
            AsyncOperationHandle localizeHandle =
                Addressables.LoadAssetAsync<LocalizeTable>("LocalizeTable");
            localizeHandle.Completed += op =>
            {
                var res = op.Result as LocalizeTable;
                _localizeTable = res;
            };
            await localizeHandle;
            Addressables.Release(localizeHandle);
            
            toolDataDic = _toolTable.TryCache();
            relicDataDic = _relicDataTable.TryCache();
            toolExcelDataDic = _toolExcelDatas.TryCache();
            toolCardOpDataDic = _toolCardOptionTable.TryCache();
            gacheInGameDataDic = _gachaTable.TryCache();
            tagDataDic = _tagTable.TryCache();
            localizeDataDic = _localizeTable.TryCache();
            
            initialized=true;
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
            if (toolExcelDataDic.TryGetValue(t, out var ret))
                return ret;
            else
            {
                Debug.LogError($"{t} tool data is undefined");
                return toolExcelDataDic[ToolType.Card_Rake];
            }
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
            tagDataDic.TryGetValue(id, out var ret);
            return ret;
        }

        public LocalizeData GetLocalizeData(string name)
        {
            localizeDataDic.TryGetValue(name, out var ret);
            return ret;
        }
    }
}