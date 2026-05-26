using UnityEngine;

namespace FluffyDisdog.CardOptionExecuter
{
    public class LineClearExecuter:CardOptionExecuter
    {
        protected override void OnPostEffect(CardExecuteParam param)
        {
            base.OnPostEffect(param);
            if (param is AfterEmulateParam emul)
            {
                if (emul.successed >= rawData.Values[0])
                {
                    Debug.Log($"12번 옵션 성공. 타격 성공 타일 : {emul.successed} 성공 필요량 {rawData.Values[0]}");
                    var list = TileGameManager.I.TileSet.GetAllNodesByDirection(rawData.Values[1], emul.target);
                    foreach (var node in list)
                    {
                        if(node.ValidNode() && !node.isObstacle)
                            node.TryDigBlockForce();
                    }
                }
            }
        }
    }
}