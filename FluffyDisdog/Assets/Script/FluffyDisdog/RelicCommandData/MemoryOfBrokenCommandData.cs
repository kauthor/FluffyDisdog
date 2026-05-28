using FluffyDisdog.Data;
using FluffyDisdog.Data.RelicData;
using Script.FluffyDisdog.Managers;

namespace FluffyDisdog.RelicCommandData
{
    public class ToolDestroyedParam : TurnEventOptionParam
    {
        public CardInGame tool;
    }
    public class MemoryOfBrokenCommandData: RelicCommandData
    {
        public override RelicName relicType => RelicName.MemoryOfBroken;

        override public void InitCommandData(RelicData data)
        {
            base.InitCommandData(data);
            PlayerManager.I.TurnEventSystem.AddEvent(TurnEvent.ToolDestroyed, ExecuteCommand, this);
        }

        protected override void OnExecuteCommand(TurnEventOptionParam param)
        {
            base.OnExecuteCommand(param);
            if (param is ToolDestroyedParam des)
            {
                var data = ExcelManager.I.GetToolExcelData(des.tool.ToolType);
                if ((data.ToolTag & ToolTag.Sixth) != 0)
                {
                    TileGameManager.I.GainAdditionalGold(1);
                }
            }
        }
    }
}