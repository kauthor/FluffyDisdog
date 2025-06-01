using FluffyDisdog.Data.RelicData;
using Script.FluffyDisdog.Managers;

namespace FluffyDisdog.RelicCommandData
{
    public class ArtisansWhetstoneCommandData: RelicCommandData
    {
        public override RelicName relicType => RelicName.ArtisansWhetstone;

        protected override void OnExecuteCommand(TurnEventOptionParam param)
        {
            base.OnExecuteCommand(param);
            PlayerManager.I.RuntimeStat.AddTileSuccessRate(rawData.Values[0]);
        }

        override public void InitCommandData(RelicData data)
        {
            base.InitCommandData(data);
            PlayerManager.I.TurnEventSystem.AddEvent(TurnEvent.GameStart, ExecuteCommand, this);
        }
    }
}