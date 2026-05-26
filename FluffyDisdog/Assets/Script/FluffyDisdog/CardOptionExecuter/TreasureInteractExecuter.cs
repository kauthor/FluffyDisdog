using UnityEngine;

namespace FluffyDisdog.CardOptionExecuter
{
    public class TreasureInteractExecuter:CardOptionExecuter
    {
        protected override void OnTryInteract(CardExecuteParam param)
        {
            base.OnTryInteract(param);
            if (param.target.isTreasure)
            {
                Debug.Log($"4번 옵션 성공.");
                AccountManager.I.AddGold(rawData.Values[0]);
            }
        }
    }
}