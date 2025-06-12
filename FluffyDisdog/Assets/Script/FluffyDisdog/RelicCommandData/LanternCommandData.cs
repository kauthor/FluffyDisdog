using FluffyDisdog.Data.RelicData;
using Script.FluffyDisdog.Managers;

namespace FluffyDisdog.RelicCommandData
{
    public class LanternCommandData:RelicCommandData
    {
//todo : 암흑 구현 후 구현
        public override RelicName relicType => RelicName.Lantern;

        public override void InitCommandData(RelicData data)
        {
            base.InitCommandData(data);
            PlayerManager.I.TurnEventSystem.AddEvent(TurnEvent.TileDigged, ExecuteCommand, this);
        }

        protected override void OnExecuteCommand(TurnEventOptionParam param)
        {
            base.OnExecuteCommand(param);
            if (param is TileDiggedParam t)
            {
                if (t.target.SubstateSystem.Is(NodeSubstate.Dark))
                {
                    TileGameManager.I.AddScore((int)rawData.Values[0]);
                }
            }
        }
    }
}
