using FluffyDisdog.Data.RelicData;
using Script.FluffyDisdog.Managers;

namespace FluffyDisdog.RelicCommandData
{
    public class AdvancedHandleCommandData:RelicCommandData
    {
        public override RelicName relicType => RelicName.AdvancedHandle;

        override public void InitCommandData(RelicData data)
        {
            base.InitCommandData(data);
            PlayerManager.I.TurnEventSystem.AddEvent(TurnEvent.GameStart, ExecuteCommand, this);
        }

        protected override void OnExecuteCommand(TurnEventOptionParam param)
        {
            base.OnExecuteCommand(param);
            PlayerManager.I.RuntimeStat.AddScoreMulti(rawData.Values[0]);
        }
    }
    
}