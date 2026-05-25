using FluffyDisdog.Data;
using Script.FluffyDisdog.Managers;

namespace FluffyDisdog.CardOptionExecuter
{
    public class DrawGoldCardExecuter:CardOptionExecuter
    {
        protected override void OnPostEffect(CardExecuteParam param)
        {
            base.OnPostEffect(param);
            var data = ExcelManager.I.GetToolExcelData(TileGameManager.I.CurrentTool);
            if (data != null)
            {
                if (param is AfterEmulateParam emul && emul.successed >= rawData.Values[0])
                {
                    for(int i=0; i< rawData.Values[1]; i++)
                       DeckManager.I.Draw(true, _ => (_.ExcelData.ToolTag & ToolTag.Seventh) >0);
                }
            }
        }
    }
}