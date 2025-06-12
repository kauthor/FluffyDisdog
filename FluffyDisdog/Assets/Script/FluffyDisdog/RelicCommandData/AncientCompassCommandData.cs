using FluffyDisdog.Data.RelicData;
using Script.FluffyDisdog.Managers;
using UnityEngine;

namespace FluffyDisdog.RelicCommandData
{
    public class TileClickedParam : TurnEventOptionParam
    {
        public TerrainNode targetNode;
    }
    
    public class AncientCompassCommandData:RelicCommandData
    {
        private int maxDestroy=1;

        private int count;
        public override RelicName relicType => RelicName.AncientCompass;

        override public void InitCommandData(RelicData data)
        {
            base.InitCommandData(data);
            PlayerManager.I.TurnEventSystem.AddEvent(TurnEvent.TileClicked, ExecuteCommand, this);
            count = 0;
        }

        protected override void OnExecuteCommand(TurnEventOptionParam param)
        {
            base.OnExecuteCommand(param);
            

            if (count < maxDestroy)
            {
                if (param is TileClickedParam aparam)
                {
                    var Owner = aparam.targetNode;
                    var nearTarget = TileGameManager.I.TileSet.GetNearTiles(Owner.Coord);
                    nearTarget.RemoveAll(_ => !_.IsDiggable());
                    if (nearTarget.Count > 0)
                    {
                        var target = Random.Range(0, nearTarget.Count);
                        nearTarget[target].TryDigBlockForce();
                    }
                    
                    PlayerManager.I.TurnEventSystem.RemoveAllEventAsType(this, TurnEvent.ToolConsumed);
                }
            }
            
            count++;
        }
    }
}
