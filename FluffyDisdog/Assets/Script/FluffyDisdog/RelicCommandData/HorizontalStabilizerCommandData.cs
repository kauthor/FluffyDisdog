using System;
using FluffyDisdog.Data.RelicData;
using Script.FluffyDisdog.Managers;

namespace FluffyDisdog.RelicCommandData
{
    public class HorizontalStabilizerCommandData:RelicCommandData
    {
        public override RelicName relicType => RelicName.HorizontalStabilizer;

        protected override void OnExecuteCommand(TurnEventOptionParam param)
        {
            base.OnExecuteCommand(param);
            if (param is TileEmulatorOptionParam horver)
            {
                if(horver.clickedCoord.Item2 == horver.targetCoord.Item2)
                   horver.addToolRate += rawData.Values[0]/10000.0f;
            }
        }

        public override void InitCommandData(RelicData data)
        {
            base.InitCommandData(data);
            PlayerManager.I.TurnEventSystem.AddEvent(TurnEvent.DistanceDesire, ExecuteCommand, this);
        }
    }
}
