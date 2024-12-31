using FluffyDisdog;

namespace Script.FluffyDisdog.TileClass
{
    public class ObstacleType1:NodeExecuter
    {
        public override void Execute()
        {
            
        }
    }
    public class ObstacleType2:NodeExecuter
    {
        public override void Execute()
        {
            
        }
    }
    public class ObstacleType3:NodeExecuter
    {
        public override void Execute()
        {
            //parentTileSet.SwapNormalTiles();
            parentTileSet.SwapAllTiles();
        }
    }
    public class ObstacleType4:NodeExecuter
    {
        public override void Execute()
        {
            
        }
    }
    public class ObstacleType5:NodeExecuter
    {
        public override void Execute()
        {
            parentTileSet.RegenRandomNormalTileAsObstacle();
        }
    }
}