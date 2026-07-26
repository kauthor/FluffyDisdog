using FluffyDisdog;
using FluffyDisdog.RelicCommandData;
using Script.FluffyDisdog.Managers;

namespace Script.FluffyDisdog.TileClass
{
    public class TreasureType1:NodeExecuter
    {
        public override void Execute()
        {
            parentTileSet.TryAddExecutedNode(node);
            TileGameManager.I.MultiplyScore(1.5f);
            node.EnableNode(false);
        }
    }
    public class TreasureType2:NodeExecuter, IEventAffectable
    {
        public override void Execute()
        {
            parentTileSet.TryAddExecutedNode(node);
            PlayerManager.I.TurnEventSystem.AddEvent(TurnEvent.ToolConsumed, ToolSafe, this);
            node.EnableNode(false);
        }

        private void ToolSafe(TurnEventOptionParam param)
        {
            if (param is ToolConsumeDesire des)
            {
                des.consumed = false;
            }
            PlayerManager.I.TurnEventSystem.RemoveEvent(this);
        }
    }
    public class TreasureType3:NodeExecuter
    {
        public override void Execute()
        {
            parentTileSet.TryAddExecutedNode(node);
            node.EnableNode(false);
            //추가 UI만 있다면 1분컷 같은데...내일 저녁에 요구해야지
        }
    }
    public class TreasureType4:NodeExecuter
    {
        public override void Execute()
        {
            parentTileSet.TryAddExecutedNode(node);
            node.EnableNode(false);
            DeckManager.I.Draw();
        }
    }
    public class TreasureType5:NodeExecuter
    {
        public override void Execute()
        {
            parentTileSet.TryAddExecutedNode(node);
            node.EnableNode(false);
            
        }
    }
}