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
            bool exist = false;
            if (data != null)
            {
                foreach (var d in data)
                {
                    if(d!=null && d.CardAddType == 13)
                        exist = true;
                }
            }
            if (exist /*&& data.Values[0] == 5*/)
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
            bool exist = false;
            if (data != null)
            {
                foreach (var d in data)
                {
                    if(d!=null && d.CardAddType == 13)
                        exist = true;
                }
            }
            if (exist /*&& data.Values[0] == 5*/)
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
    public class ObstacleType3:NodeExecuter,IEventAffectable
    {
        public override void Execute()
        {
            var type = TileGameManager.I.CurrentTool;
            var data = ExcelManager.I.GetToolCardOpData(type);
            bool exist = false;
            if (data != null)
            {
                foreach (var d in data)
                {
                    if(d!=null && d.CardAddType == 13)
                        exist = true;
                }
            }
            if (exist /*&& data.Values[0] == 5*/)
            {
                parentTileSet.TryAddExecutedNode(node);
                node.EnableNode(false);
            }
            else
            {
                //여기서 장애물로 기능
                PlayerManager.I.TurnEventSystem.AddEvent(TurnEvent.TurnEnd, TurnEnd,this);
                
            }
            //parentTileSet.SwapNormalTiles();
            
        }

        private void TurnEnd(TurnEventOptionParam param)
        {
            parentTileSet.SwapAllTiles();
            PlayerManager.I.TurnEventSystem.RemoveEvent(this);
        }
    }
    public class ObstacleType4:NodeExecuter
    {
        public override void Execute()
        {
            var type = TileGameManager.I.CurrentTool;
            var data = ExcelManager.I.GetToolCardOpData(type);
            bool exist = false;
            if (data != null)
            {
                foreach (var d in data)
                {
                    if(d!=null && d.CardAddType == 13)
                        exist = true;
                }
            }
            if (exist /*&& data.Values[0] == 5*/)
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
            bool exist = false;
            if (data != null)
            {
                foreach (var d in data)
                {
                    if(d!=null && d.CardAddType == 13)
                        exist = true;
                }
            }
            if (exist /*&& data.Values[0] == 5*/)
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