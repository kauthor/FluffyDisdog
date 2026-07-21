using FluffyDisdog.Data;
using FluffyDisdog.Data.RelicData;
using Script.FluffyDisdog.Managers;

namespace FluffyDisdog.RelicCommandData
{
    public class ArtisansWhetstoneCommandData: RelicCommandData
    {
        public override RelicName relicType => RelicName.ArtisansWhetstone;

        protected override void OnExecuteCommand(TurnEventOptionParam param)
        {
            base.OnExecuteCommand(param);
            if (param is TileEmulatorOptionParam emul)
            {
                var data = ExcelManager.I.GetToolExcelData(emul.toolType);
                if ( (data.ToolTag & ToolTag.Sixth) != 0)
                {
                    emul.addToolRate += rawData.Values[0];
                }
            }
        }

        override public void InitCommandData(RelicData data)
        {
            base.InitCommandData(data);
            PlayerManager.I.TurnEventSystem.AddEvent(TurnEvent.ToolCalculateStart , ExecuteCommand, this);
        }
    }
}