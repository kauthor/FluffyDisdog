using UnityEngine;

namespace FluffyDisdog.CardOptionExecuter
{
    public class ObstacleCrushExecuter:CardOptionExecuter
    {
        protected override void OnSuccess(CardExecuteParam param)
        {
            base.OnSuccess(param);
            var rate = Random.Range(0, 10000);
            if (rate > rawData.Values[1])
                return;
            if (param.target.isObstacle )
            {
                AccountManager.I.AddGold(rawData.Values[0]);
            }
        }
    }
}