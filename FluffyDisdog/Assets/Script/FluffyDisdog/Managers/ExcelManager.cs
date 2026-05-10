using System.Collections.Generic;
using System.Linq;
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
        private Dictionary<int, GachaData> gachaDataDic;
        private Dictionary<int, TagData> tagDataDic;
        private Dictionary<string, LocalizeData> localizeDataDic;
        
        /// <summary>
        /// 26.05.10
        /// </summary>
        
        private PackTable _packTable;
        private ShopItemTable _shopItemTable;
        private ShopTable _shopTable;
        private MapTable _mapTable;
        private RequestTable _requestTable;
        private BoxTable _boxTable;
        private BoxItemTable _boxItemTable;
        
        private Dictionary<string, PackData> packDataDic;
        private Dictionary<string, ShopItemData> shopItemDataDic;
        private Dictionary<int, ShopData> shopDataDic;
        private Dictionary<int, MapData> mapDataDic;
        private Dictionary<int, RequestData> requestDataDic;
        private Dictionary<int, BoxData> boxDataDic;
        private List<BoxItemData> boxItemDataDic;
        
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
            
            AsyncOperationHandle packHandle =
                Addressables.LoadAssetAsync<PackTable>("PackTable");
            packHandle.Completed += op =>
            {
                var res = op.Result as PackTable;
                _packTable = res;
            };
            await packHandle;
            Addressables.Release(packHandle);
            
            AsyncOperationHandle shopItemHandle =
                Addressables.LoadAssetAsync<ShopItemTable>("ShopItemTable");
            shopItemHandle.Completed += op =>
            {
                var res = op.Result as ShopItemTable;
                _shopItemTable = res;
            };
            await shopItemHandle;
            Addressables.Release(shopItemHandle);
            
            AsyncOperationHandle shopHandle =
                Addressables.LoadAssetAsync<ShopTable>("ShopTable");
            shopHandle.Completed += op =>
            {
                var res = op.Result as ShopTable;
                _shopTable = res;
            };
            await shopHandle;
            Addressables.Release(shopHandle);
            
            AsyncOperationHandle maphandle =
                Addressables.LoadAssetAsync<MapTable>("MapTable");
            maphandle.Completed += op =>
            {
                var res = op.Result as MapTable;
                _mapTable = res;
            };
            await maphandle;
            Addressables.Release(maphandle);
            
            AsyncOperationHandle requestHandle =
                Addressables.LoadAssetAsync<RequestTable>("RequestTable");
            requestHandle.Completed += op =>
            {
                var res = op.Result as RequestTable;
                _requestTable = res;
            };
            await requestHandle;
            Addressables.Release(requestHandle);
            
            AsyncOperationHandle boxhandle =
                Addressables.LoadAssetAsync<BoxTable>("BoxTable");
            boxhandle.Completed += op =>
            {
                var res = op.Result as BoxTable;
                _boxTable = res;
            };
            await boxhandle;
            Addressables.Release(boxhandle);
            
            AsyncOperationHandle boxItemHandle =
                Addressables.LoadAssetAsync<BoxItemTable>("BoxItemTable");
            boxItemHandle.Completed += op =>
            {
                var res = op.Result as BoxItemTable;
                _boxItemTable = res;
            };
            await boxItemHandle;
            Addressables.Release(boxItemHandle);
            
            toolDataDic = _toolTable.TryCache();
            relicDataDic = _relicDataTable.TryCache();
            toolExcelDataDic = _toolExcelDatas.TryCache();
            toolCardOpDataDic = _toolCardOptionTable.TryCache();
            gacheInGameDataDic = _gachaTable.TryCacheGachaGroup();
            tagDataDic = _tagTable.TryCache();
            localizeDataDic = _localizeTable.TryCache();
            gachaDataDic = _gachaTable.TryCacheData();
            
            packDataDic = _packTable.TryCache();
            shopDataDic = _shopTable.TryCache();
            shopItemDataDic = _shopItemTable.TryCache();
            mapDataDic = _mapTable.TryCache();
            requestDataDic = _requestTable.TryCache();
            boxDataDic = _boxTable.TryCache();
            boxItemDataDic = _boxItemTable.TryCache();
            
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

        public PackData GetPackData(string name)
        {
            packDataDic.TryGetValue(name, out var ret);
            return ret;
        }

        public ShopItemData GetShopItemData(string name)
        {
            shopItemDataDic.TryGetValue(name, out var ret);
            return ret;
        }

        public ShopData GetShopData(int key)
        {
            shopDataDic.TryGetValue(key, out var ret);
            return ret;
        }

        public MapData GetMapData(int key)
        {
            mapDataDic.TryGetValue(key, out var ret);
            return ret;
        }

        public RequestData GetRequestData(int key)
        {
            requestDataDic.TryGetValue(key, out var ret);
            return ret;
        }

        public BoxData GetBoxData(int key)
        {
            boxDataDic.TryGetValue(key, out var ret);
            return ret;
        }

        public BoxItemData[] GetBoxItemData(int key)
        {
            var ret = boxItemDataDic.Where(_=>_.boxId ==key);
            return ret.ToArray();
        }

        public GachaData ExecuteGacha(int gachaId)
        {
            if (gacheInGameDataDic.TryGetValue(gachaId, out var gacha))
            {
                int gachaPoint = Random.Range(0, gacha.rateSum);
                int result = 0;

                for (int i = 0; i < gacha.rateArray.Length; i++)
                {
                    if (gacha.rateArray[i] >= gachaPoint)
                    {
                        result = i;
                        break;
                    }
                }
                int key = gacha.key[result];

                var ret = gachaDataDic[key];
                return ret;
            }
            return null;
        }
    }
}