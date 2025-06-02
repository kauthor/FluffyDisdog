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

        protected override void OnExecuteCommand(TurnEventOptionParam param)
        {
            base.OnExecuteCommand(param);
            if (param is ToolCalculateStart cal)
            {
                var data = ExcelManager.I.GetToolData(cal.toolType);
                if (data.cellHeight == 1 && data.cellWidth == 1)
                {
                    cal.addRate += rawData.Values[0];
                }
            }
           
        }

        public override void InitCommandData(RelicData data)
        {
            base.InitCommandData(data);
            PlayerManager.I.TurnEventSystem.AddEvent(TurnEvent.ToolCalculateStart, ExecuteCommand, this);
        }
    }
}