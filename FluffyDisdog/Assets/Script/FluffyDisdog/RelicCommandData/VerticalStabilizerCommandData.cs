using FluffyDisdog.Data.RelicData;
using Script.FluffyDisdog.Managers;

namespace FluffyDisdog.RelicCommandData
{
    public class VerticalStabilizerCommandData:RelicCommandData
    {
        public override RelicName relicType => RelicName.VerticalStabilizer;

        protected override void OnExecuteCommand(TurnEventOptionParam param)
        {
            base.OnExecuteCommand(param);
            if (param is TileEmulatorOptionParam horver)
            {
                if(horver.clickedCoord.Item1 == horver.targetCoord.Item1)
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