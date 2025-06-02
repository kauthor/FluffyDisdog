using FluffyDisdog.Data.RelicData;
using Script.FluffyDisdog.Managers;

namespace FluffyDisdog.RelicCommandData
{
    public class RiftShardEnhancerCommandData:RelicCommandData
    {
        public override RelicName relicType => RelicName.RiftShardEnhancer;

        protected override void OnExecuteCommand(TurnEventOptionParam param)
        {
            base.OnExecuteCommand(param);
            if (param is TileDiggedParam tile)
            {
                if (tile.target.SubstateSystem.Is(NodeSubstate.Crack))
                {
                    TileGameManager.I.AddScore((int)rawData.Values[0]);
                }
            }
        }

        public override void InitCommandData(RelicData data)
        {
            base.InitCommandData(data);
            PlayerManager.I.TurnEventSystem.AddEvent(TurnEvent.TileDigged, ExecuteCommand, this);
        }
    }
}
