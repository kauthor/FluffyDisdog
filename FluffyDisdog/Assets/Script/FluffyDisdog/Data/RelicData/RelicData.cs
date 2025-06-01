using System;
using UnityEngine;

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

        public RelicData()
        {
            
        }
        
        #if UNITY_EDITOR
        public RelicData(TurnEvent newEvent, RelicName name, float[] val)
        {
            relicName = name;
            values = val;
        }
        #endif
    }
}