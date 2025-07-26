namespace FluffyDisdog.CardOptionExecuter
{
    public class RandomExecuter:CardOptionExecuter
    {
        protected override void OnPreEffect(CardExecuteParam param)
        {
            base.OnPreEffect(param);
            int amount = rawData.Values[0];
            for (int i = 0; i < amount; i++)
            {
                var tile = TileGameManager.I.TileSet.GetRandomNode(_=>_.CurrentState == NodeState.Raw && !_.isObstacle && !_.isTreasure);
                tile.TryDigBlockForce();
            }
        }
    }
}