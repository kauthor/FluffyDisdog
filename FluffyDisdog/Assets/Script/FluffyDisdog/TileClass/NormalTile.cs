using FluffyDisdog;

namespace Script.FluffyDisdog.TileClass
{
    public class NormalTile:NodeExecuter
    {
        public override void Execute()
        {
            //노멀은 아무것도 안한다
            parentTileSet.TryAddExecutedNode(node);
            node.EnableNode(false);
        }
    }
}