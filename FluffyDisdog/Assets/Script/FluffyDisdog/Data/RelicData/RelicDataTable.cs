using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace FluffyDisdog.Data.RelicData
{
    public enum RelicName
    {
        NONE=0,
        Oil=101,
        AdvancedHandle=102,
        ArtisansWhetstone=103,
        MemoryOfBroken=104,
        ImmutableCoating=105,
        SmallGoldIngot=106,
        ExpandedBackpack=107,
        ToolPouch=108,
        PrecisionStrikeGuide=109,
        AreaAttackSpecialist=110,
        EmergencyRepairKit=111,
        AncientCompass=112,
        Lantern=113,
        RuinsSurveyNotes=114,
        TradersCompanion=115,
        RiftGenerator=116,
        RiftShardEnhancer=117,
        PerfectToolbox=118,
        AfterimageTracker=119,
        AfterimageEngraving=120,
        PrecisionScope=121,
        HorizontalVerticalStabilizer=122
    }
    [Serializable]
    public class RelicData
    {
        //public TurnEvent eventType;

        public string localKey;
        
        public RelicName relicName;

        [SerializeField] private float[] values;
        
        public float[] Values => values;

        public RelicData(int relicId, float[] val, string key)
        {
            relicName = (RelicName)relicId;
            values = val;
            localKey = key;
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