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

    public static class RelicNameExtension
    {
        public static RelicName StringToRelicName(this string relicName)
        {
            var newStr = relicName.Replace("relic","");
            int toInt = int.Parse(newStr);
            return (RelicName)toInt;
        }
    }
    [Serializable]
    public class RelicData
    {
        //public TurnEvent eventType;

        public string localKey;
        public string localDesc;
        public int relicRarity;
        
        public RelicName relicName;

        [SerializeField] private float[] values;
        
        public float[] Values => values;

        public RelicData(int relicId, float[] val, string nameKey, string descKey, int rarity)
        {
            relicName = (RelicName)relicId;
            values = val;
            localKey = nameKey;
            localDesc = descKey;
            relicRarity = rarity;
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