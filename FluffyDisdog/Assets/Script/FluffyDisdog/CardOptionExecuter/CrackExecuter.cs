using UnityEngine;

namespace FluffyDisdog.CardOptionExecuter
{
    public class CrackExecuter:CardOptionExecuter
    {
        protected override void OnExecuteTileEffect(CardExecuteParam param)
        {
            base.OnExecuteTileEffect(param);
            var rate = Random.Range(0, 10000);
            if (rate <= rawData.Values[0])
            {
                param.target.SubstateSystem.SetState(NodeSubstate.Crack);
                Debug.Log($"2번 옵션 성공. 성공률 {rawData.Values[0]}");
            }
            TileGameManager.I.TileSet.HitFail(param.target);
        }
    }
}