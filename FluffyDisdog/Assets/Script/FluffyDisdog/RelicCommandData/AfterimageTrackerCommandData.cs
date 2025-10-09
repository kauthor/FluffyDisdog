using FluffyDisdog.Data.RelicData;
using Script.FluffyDisdog.Managers;

namespace FluffyDisdog.RelicCommandData
{
    public class AfterimageTrackerCommandData:RelicCommandData
    {
        public override RelicName relicType => RelicName.AfterimageTracker;

        public override void InitCommandData(RelicData data)
        {
            base.InitCommandData(data);
            PlayerManager.I.TurnEventSystem.AddEvent(TurnEvent.TileClicked, ExecuteCommand, this);
            PlayerManager.I.TurnEventSystem.AddEvent(TurnEvent.TurnEnd, TurnEnd, this);
        }

        private bool hasAdded = false;

        protected override void OnExecuteCommand(TurnEventOptionParam param)
        {
            base.OnExecuteCommand(param);
            if (param is TileClickedParam tparam)
            {
                if (tparam.targetNode.CurrentState == NodeState.AfterImage)
                {
                    PlayerManager.I.RuntimeStat.AddScoreMulti(rawData.Values[0]);
                    hasAdded = true;
                }
            }
           
        }

        private void TurnEnd(TurnEventOptionParam param)
        {
            if(hasAdded)
               PlayerManager.I.RuntimeStat.AddScoreMulti(-rawData.Values[0]);
            
            hasAdded = false;
        }
    }
}
