namespace FluffyDisdog.CardOptionExecuter
{
    public class ObstacleInteractExecuter:CardOptionExecuter
    {
        protected override void OnTryInteract(CardExecuteParam param)
        {
            base.OnTryInteract(param);
            if (param.target.isObstacle && param.target.ObstacleType == (ObstacleType)rawData.Values[0])
            {
                AccountManager.I.AddGold(rawData.Values[1]);
            }
        }
    }
}