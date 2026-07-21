using FluffyDisdog.Data.RelicData;
using Script.FluffyDisdog.Managers;
using UnityEngine;

namespace FluffyDisdog.RelicCommandData
{
    public class RiftGeneratorCommandData : RelicCommandData
    {
        public override RelicName relicType => RelicName.RiftGenerator;

        protected override void OnExecuteCommand(TurnEventOptionParam param)
        {
            base.OnExecuteCommand(param);
            if (param is TileEmulatorOptionParam dig)
            {
                var rand = Random.Range(0, 10000);
                if(rand < rawData.Values[0])
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
