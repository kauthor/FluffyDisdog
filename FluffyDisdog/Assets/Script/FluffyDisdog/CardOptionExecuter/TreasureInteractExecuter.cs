namespace FluffyDisdog.CardOptionExecuter
{
    public class TreasureInteractExecuter:CardOptionExecuter
    {
        protected override void OnTryInteract(CardExecuteParam param)
        {
            base.OnTryInteract(param);
            if (param.target.isTreasure)
            {
                AccountManager.I.AddGold(rawData.Values[0]);
            }
        }
    }
}