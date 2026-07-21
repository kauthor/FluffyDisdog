using UnityEngine;

namespace FluffyDisdog.CardOptionExecuter
{
    public class ObstacleInteractExecuter:CardOptionExecuter
    {
        protected override void OnTryInteract(CardExecuteParam param)
        {
            base.OnTryInteract(param);
            if (param.target.isObstacle && param.target.ObstacleType == (ObstacleType)rawData.Values[0])
            {
                Debug.Log($"5번 옵션 성공. 타입 {(ObstacleType)rawData.Values[0]}");
                AccountManager.I.AddGold(rawData.Values[1]);
            }
        }
    }
}