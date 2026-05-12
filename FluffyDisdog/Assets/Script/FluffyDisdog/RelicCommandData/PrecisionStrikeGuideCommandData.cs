using FluffyDisdog.Data.RelicData;
using Script.FluffyDisdog.Managers;

namespace FluffyDisdog.RelicCommandData
{
    public class ToolCalculateStart : TurnEventOptionParam
    {
        public ToolType toolType;
        public float addRate;
    }
    public class PrecisionStrikeGuideCommandData:RelicCommandData
    {
        public override RelicName relicType => RelicName.PrecisionStrikeGuide;

        private bool hasAdded = false;
        
        
        protected override void OnExecuteCommand(TurnEventOptionParam param)
        {
            base.OnExecuteCommand(param);
            if (param is ToolCalculateStart cal)
            {
                var data = ExcelManager.I.GetToolData(cal.toolType);
                if (data.cellHeight == 1 && data.cellWidth == 1)
                {
                    PlayerManager.I.RuntimeStat.AddScoreMulti(rawData.Values[0]-1);
                    hasAdded = true;
                }
            }
           
        }

        private void TurnEnd(TurnEventOptionParam param)
        {
            if(hasAdded)
                PlayerManager.I.RuntimeStat.AddScoreMulti(-rawData.Values[0]+1);
            
            hasAdded = false;
        }
        public override void InitCommandData(RelicData data)
        {
            base.InitCommandData(data);
            PlayerManager.I.TurnEventSystem.AddEvent(TurnEvent.ToolCalculateStart, ExecuteCommand, this);
            PlayerManager.I.TurnEventSystem.AddEvent(TurnEvent.TurnEnd, TurnEnd, this);
        }
    }
}