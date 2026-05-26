using FluffyDisdog;
using Script.FluffyDisdog.Managers;

namespace Script.FluffyDisdog.TileClass
{
    /// <summary>
    /// todo:여기 조건문은 cardAddData 임포트 후에 없앤다.
    /// </summary>
    public class ObstacleType1:NodeExecuter
    {
        public override void Execute()
        {
            var type = TileGameManager.I.CurrentTool;
            var data = ExcelManager.I.GetToolCardOpData(type);
            if (data.CardAddType == 13 && data.Values[0] == 1)
            {
                parentTileSet.TryAddExecutedNode(node);
                node.EnableNode(false);
            }
            else
            {
                //여기서 장애물로 기능
            }
        }
    }
    public class ObstacleType2:NodeExecuter
    {
        public override void Execute()
        {
            var type = TileGameManager.I.CurrentTool;
            var data = ExcelManager.I.GetToolCardOpData(type);
            if (data.CardAddType == 13 && data.Values[0] == 2)
            {
                parentTileSet.TryAddExecutedNode(node);
                node.EnableNode(false);
            }
            else
            {
                //여기서 장애물로 기능
            }
        }
    }
    public class ObstacleType3:NodeExecuter
    {
        public override void Execute()
        {
            var type = TileGameManager.I.CurrentTool;
            var data = ExcelManager.I.GetToolCardOpData(type);
            if (data.CardAddType == 13 && data.Values[0] == 3)
            {
                parentTileSet.TryAddExecutedNode(node);
                node.EnableNode(false);
            }
            else
            {
                //여기서 장애물로 기능
                parentTileSet.SwapAllTiles();
            }
            //parentTileSet.SwapNormalTiles();
            
        }
    }
    public class ObstacleType4:NodeExecuter
    {
        public override void Execute()
        {
            var type = TileGameManager.I.CurrentTool;
            var data = ExcelManager.I.GetToolCardOpData(type);
            if (data.CardAddType == 13 && data.Values[0] == 4)
            {
                parentTileSet.TryAddExecutedNode(node);
                node.EnableNode(false);
            }
            else
            {
                //여기서 장애물로 기능
                
            }
        }
    }
    public class ObstacleType5:NodeExecuter
    {
        public override void Execute()
        {
            var type = TileGameManager.I.CurrentTool;
            var data = ExcelManager.I.GetToolCardOpData(type);
            if (data.CardAddType == 13 && data.Values[0] == 5)
            {
                parentTileSet.TryAddExecutedNode(node);
                node.EnableNode(false);
            }
            else
            {
                //여기서 장애물로 기능
                parentTileSet.RegenRandomNormalTileAsObstacle();
            }
            
        }
    }
}