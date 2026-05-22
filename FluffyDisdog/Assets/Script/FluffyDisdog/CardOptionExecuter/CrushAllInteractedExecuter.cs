using System.Collections.Generic;

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