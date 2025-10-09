using FluffyDisdog.Data.RelicData;
using Script.FluffyDisdog.Managers;

namespace FluffyDisdog.RelicCommandData
{
    public class AfterimageEngravingCommandData:RelicCommandData
    {
        public override RelicName relicType => RelicName.AfterimageEngraving;

        public override void InitCommandData(RelicData data)
        {
            base.InitCommandData(data);
            PlayerManager.I.TurnEventSystem.AddEvent(TurnEvent.GameStart, ExecuteCommand, this);
        }

        protected override void OnExecuteCommand(TurnEventOptionParam param)
        {
            base.OnExecuteCommand(param);
            for (int i = 0; i < rawData.Values[0]; i++)
            {
                var rand = TileGameManager.I.TileSet.GetRandomNode(_ =>
                    _.CurrentState == NodeState.Raw && !_.isObstacle && !_.isTreasure);
                rand.EnableNode(false);
            }
        }
    }
}
