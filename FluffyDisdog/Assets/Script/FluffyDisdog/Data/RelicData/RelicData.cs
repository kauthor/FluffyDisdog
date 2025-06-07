using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace FluffyDisdog.Data.RelicData
{
    public enum RelicName
    {
        NONE=0,
        Oil=1,
        AdvancedHandle=2,
        ArtisansWhetstone=3,
        MemoryOfBroken=4,
        ImmutableCoating=5,
        SmallGoldIngot=6,
        ExpandedBackpack,
        ToolPouch,
        PrecisionStrikeGuide,
        AreaAttackSpecialist,
        EmergencyRepairKit,
        AncientCompass,
        Lantern,
        RuinsSurveyNotes,
        TradersCompanion,
        RiftGenerator,
        RiftShardEnhancer,
        PerfectToolbox,
        AfterimageTracker,
        AfterimageEngraving,
        PrecisionScope,
        HorizontalVerticalStabilizer
    }
    [Serializable]
    public class RelicData
    {
        //public TurnEvent eventType;
        
        public RelicName relicName;

        [SerializeField] private float[] values;
        
        public float[] Values => values;

        public RelicData(int relicId, float[] val)
        {
            relicName = (RelicName)relicId;
            values = val;
        }
        
        
        #if UNITY_EDITOR
        public RelicData(TurnEvent newEvent, RelicName name, float[] val)
        {
            relicName = name;
            values = val;
        }
        #endif
    }
    
    [CreateAssetMenu]
    public class RelicDataTable:ScriptableObject
    {
        [SerializeField] private RelicData[] _relicDatas;

        public void SetRelicData(RelicData[] arr) => _relicDatas = arr;
        
        public RelicData GetRelicData(int index)
        {
            if (index-1 >= _relicDatas.Length)
                return _relicDatas[0];

            return _relicDatas[index-1];
        }

        public Dictionary<RelicName, RelicData> TryCache()
        {
            var ret = new Dictionary<RelicName, RelicData>();
            for (int i = 0; i < _relicDatas.Length; i++)
            {
                ret.Add(_relicDatas[i].relicName, _relicDatas[i]);
            }

            return ret;
        }
    }
}