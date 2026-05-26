using FluffyDisdog.Data;
using Script.FluffyDisdog.Managers;
using UnityEngine;

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
                    Debug.Log($"11번 옵션 성공. 타격 성공 타일 : {emul.successed} 성공 필요량 {rawData.Values[0]}");
                    for(int i=0; i< rawData.Values[1]; i++)
                       DeckManager.I.Draw(true, _ => (_.ExcelData.ToolTag & ToolTag.Seventh) >0);
                }
            }
        }
    }
}