using FluffyDisdog.Data;
using FluffyDisdog.Data.RelicData;
using Script.FluffyDisdog.Managers;

namespace FluffyDisdog.RelicCommandData
{
    public class DrawParam : TurnEventOptionParam
    {
        public ToolType toolType;
    }
    public class PerfectToolboxCommandData:RelicCommandData
    {
//todo: 강화 물어본 후 작업
        public override RelicName relicType => RelicName.PerfectToolbox;

        private ToolTag targetTag => ToolTag.Ninth;

        protected override void OnExecuteCommand(TurnEventOptionParam param)
        {
            base.OnExecuteCommand(param);
            if (param is DrawParam draw)
            {
                var data = ExcelManager.I.GetToolData(draw.toolType);
                if ((data.tag & targetTag) != 0)
                {
                    AccountManager.I.AddGold((int)rawData.Values[0]);
                }
            }
        }

        public override void InitCommandData(RelicData data)
        {
            base.InitCommandData(data);
            PlayerManager.I.TurnEventSystem.AddEvent(TurnEvent.Draw, ExecuteCommand, this);
            
        }
    }
}
