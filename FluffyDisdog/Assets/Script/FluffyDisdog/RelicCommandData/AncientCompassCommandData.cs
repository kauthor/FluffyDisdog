using System.Collections.Generic;
using FluffyDisdog.Data.RelicData;
using Script.FluffyDisdog.Managers;
using UnityEngine;

namespace FluffyDisdog.RelicCommandData
{
    public class TileClickedParam : TurnEventOptionParam
    {
        public TerrainNode targetNode;
        public List<TerrainNode> executedNodes = new List<TerrainNode>();
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
                    var calParam = new TileEmulatorOptionParam()
                    {
                
                    };
                    PlayerManager.I.TurnEventSystem.FireEvent(TurnEvent.ToolCalculateStart, calParam);
                    if (nearTarget.Count > 0)
                    {
                        var target = Random.Range(0, nearTarget.Count);
                        nearTarget[target].TryDigBlockForce();
                        TileGameManager.I.TileSet.ShowAndGainScore(calParam, nearTarget[target]);
                        aparam.executedNodes.Add(nearTarget[target]);
                    }
                    
                    PlayerManager.I.TurnEventSystem.RemoveAllEventAsType(this, TurnEvent.TileClicked);
                }
            }
            
            count++;
        }
    }
}
