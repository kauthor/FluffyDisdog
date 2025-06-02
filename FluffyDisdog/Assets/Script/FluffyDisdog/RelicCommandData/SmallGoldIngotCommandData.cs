using FluffyDisdog.Data.RelicData;
using Script.FluffyDisdog.Managers;

namespace FluffyDisdog.RelicCommandData
{
    public class SmallGoldIngotCommandData:RelicCommandData
    {
        public override RelicName relicType => RelicName.SmallGoldIngot;

        public override void InitCommandData(RelicData data)
        {
            base.InitCommandData(data);
            PlayerManager.I.TurnEventSystem.AddEvent(TurnEvent.GameStart, ExecuteCommand, this);
        }

        protected override void OnExecuteCommand(TurnEventOptionParam param)
        {
            base.OnExecuteCommand(param);
            TileGameManager.I.AddScore((int)(AccountManager.I.Gold * rawData.Values[0]));
        }
    }
}