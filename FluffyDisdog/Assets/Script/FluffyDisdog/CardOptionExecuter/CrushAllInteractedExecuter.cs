using System.Collections.Generic;
using UnityEngine;

namespace FluffyDisdog.CardOptionExecuter
{
    public class AfterEmulateParam : CardExecuteParam
    {
        public List<TerrainNode> EmulateFailed;
        public int successed;
        public AfterEmulateParam(TerrainNode target, List<TerrainNode> emulateFailed, int success)
        {
            this.EmulateFailed = emulateFailed;
            this.target = target;
            this.successed = success;
        }
    }
    public class CrushAllInteractedExecuter: CardOptionExecuter
    {
        protected override void OnPostEffect(CardExecuteParam param)
        {
            base.OnPostEffect(param);
            if (param is AfterEmulateParam emul)
            {
                if (rawData.Values[0] <= emul.successed)
                {
                    Debug.Log($"10번 옵션 성공. 타격 성공 타일 : {emul.successed} 성공 필요량 {rawData.Values[0]}");
                    foreach (TerrainNode n in emul.EmulateFailed)
                    {
                        if(n.ValidNode() && !n.isObstacle)
                            n.TryDigBlockForce();
                    }
                }
            }
            
        }
    }
}