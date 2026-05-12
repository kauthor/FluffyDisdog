using FluffyDisdog.Data.RelicData;
using Script.FluffyDisdog.Managers;

namespace FluffyDisdog.RelicCommandData
{
    public class TileDiggedParam : TurnEventOptionParam
    {
        public TerrainNode target;
    }
    public class OilCommandData:RelicCommandData
    {
        public override RelicName relicType => RelicName.Oil;

        private bool hasAdd = false;
        override public void InitCommandData(RelicData data)
        {
            base.InitCommandData(data);
            PlayerManager.I.TurnEventSystem.AddEvent(TurnEvent.ToolCalculateStart, ExecuteCommand, this);
            PlayerManager.I.TurnEventSystem.AddEvent(TurnEvent.TurnEnd, TurnEnd, this);
        }
        protected override void OnExecuteCommand (TurnEventOptionParam param)
        {
            if(!hasAdd)
            if (rawData != null && rawData.Values.Length > 0)
            {
                PlayerManager.I.RuntimeStat.AddScoreAdd((int)rawData.Values[0]);
                hasAdd = true;
            }
        }

        private void TurnEnd(TurnEventOptionParam param)
        {
            if (hasAdd)
            {
                PlayerManager.I.RuntimeStat.AddScoreAdd(-(int)rawData.Values[0]);
                hasAdd = false;
            }
        }
    }
}