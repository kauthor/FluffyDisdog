using System;
using System.Collections.Generic;
using FluffyDisdog.Data.RelicData;
using Script.FluffyDisdog.Managers;

namespace FluffyDisdog.RelicCommandData
{
    public abstract class RelicCommandData: IEventAffectable
    {
        protected RelicData rawData;
        public RelicData RawData => rawData;
        
        private static readonly Dictionary<RelicName, Func<RelicCommandData>> factoryMap =
            new Dictionary<RelicName, Func<RelicCommandData>>
            {
                { RelicName.Oil, () => new OilCommandData() },
                { RelicName.AdvancedHandle, () => new AdvancedHandleCommandData() },
                { RelicName.ArtisansWhetstone, () => new ArtisansWhetstoneCommandData() },
                { RelicName.MemoryOfBroken, () => new MemoryOfBrokenCommandData() },
                { RelicName.ImmutableCoating, () => new ImmutableCoatingCommandData() },
                { RelicName.SmallGoldIngot, () => new SmallGoldIngotCommandData() },
                { RelicName.ExpandedBackpack, () => new ExpandedBackpackCommandData() },
                { RelicName.ToolPouch, () => new ToolPouchCommandData() },
                { RelicName.PrecisionStrikeGuide, () => new PrecisionStrikeGuideCommandData() },
                { RelicName.AreaAttackSpecialist, () => new AreaAttackSpecialistCommandData() },
                { RelicName.EmergencyRepairKit, () => new EmergencyRepairKitCommandData() },
                { RelicName.AncientCompass, () => new AncientCompassCommandData() },
                { RelicName.Lantern, () => new LanternCommandData() },
                { RelicName.RuinsSurveyNotes, () => new RuinsSurveyNotesCommandData() },
                { RelicName.TradersCompanion, () => new TradersCompanionCommandData() },
                { RelicName.RiftGenerator, () => new RiftGeneratorCommandData() },
                { RelicName.RiftShardEnhancer, () => new RiftShardEnhancerCommandData() },
                { RelicName.PerfectToolbox, () => new PerfectToolboxCommandData() },
                { RelicName.AfterimageTracker, () => new AfterimageTrackerCommandData() },
                { RelicName.AfterimageEngraving, () => new AfterimageEngravingCommandData() },
                { RelicName.PrecisionScope, () => new PrecisionScopeCommandData() },
                { RelicName.HorizontalVerticalStabilizer, () => new HorizontalVerticalStabilizerCommandData() },
            };

        public abstract RelicName relicType { get; }

        //protected PlayerManager player;
        public virtual void InitCommandData(RelicData data)
        {
            rawData = data;
        }

        protected void ExecuteCommand(TurnEventOptionParam param)
        {
            OnExecuteCommand(param);
        }

        protected virtual void OnExecuteCommand(TurnEventOptionParam param)
        {
            
        }
        
        public static RelicCommandData MakeRelicCommandData(RelicData rawData)
        {
            if (factoryMap.TryGetValue(rawData.relicName, out var creator))
            {
                var instance = creator();
                instance.InitCommandData(rawData);
                return instance;
            }

            // fallback to default
            var fallback = new OilCommandData();
            fallback.InitCommandData(rawData);
            return fallback;
        }
    }
}