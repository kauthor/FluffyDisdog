using System;
using Random = UnityEngine.Random;

namespace FluffyDisdog.CardOptionExecuter
{
    public class RandomExecuter:CardOptionExecuter
    {
        protected override void OnPreEffect(CardExecuteParam param)
        {
            base.OnPreEffect(param);
            int amount = //rawData.Values[0];
                int.Parse( rawData.Desc);
            if (amount > TileGameManager.I.TileSet.ValidNodeCount)
                amount = TileGameManager.I.TileSet.ValidNodeCount;
            for (int i = 0; i < amount; i++)
            {
                if (Random.Range(0, 10000) < rawData.Values[0])
                {
                    var tile = TileGameManager.I.TileSet.GetRandomNode(_=>_.ValidNode()&&!_.isObstacle);
                    tile.TryDigBlockForce();
                }
                
            }
        }
    }
}