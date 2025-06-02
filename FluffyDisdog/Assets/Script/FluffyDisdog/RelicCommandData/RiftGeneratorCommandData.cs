using FluffyDisdog.Data.RelicData;
using Script.FluffyDisdog.Managers;

namespace FluffyDisdog.RelicCommandData
{
    public class DigFailParam : TurnEventOptionParam
    {
        public TerrainNode target;
    }
    public class RiftGeneratorCommandData : RelicCommandData
    {
        public override RelicName relicType => RelicName.RiftGenerator;

        protected override void OnExecuteCommand(TurnEventOptionParam param)
        {
            base.OnExecuteCommand(param);
            if (param is DigFailParam dig)
            {
                dig.target.SubstateSystem.SetState(NodeSubstate.Crack);
            }
        }

        public override void InitCommandData(RelicData data)
        {
            base.InitCommandData(data);
            PlayerManager.I.TurnEventSystem.AddEvent(TurnEvent.DigFail, ExecuteCommand , this);
        }
    }
}
