using FluffyDisdog;
using UnityEngine;

namespace Script.FluffyDisdog.TileClass
{
    public abstract class NodeSubstateCommand
    {
        public abstract NodeSubstate SubstateType { get; }

        public abstract void Execute();

        protected virtual void InitHandler()
        {
            
        }

        protected TerrainNode Owner;

        private NodeSubstateCommand InitCommand(TerrainNode owner)
        {
            Owner = owner;
            InitHandler();
            return this;
        }

        public static NodeSubstateCommand MakeSubstateCommand(NodeSubstate sub, TerrainNode owner)
        {
            NodeSubstateCommand ret;
            switch (sub)
            {
                case NodeSubstate.Infest:
                    ret = new NodeInfestCommand().InitCommand(owner);
                    break;
                case NodeSubstate.Crack:
                default:
                    ret = new NodeCrackCommand().InitCommand(owner);
                    break;
            }

            return ret;
        }
        
        public virtual void Finish(){}
    }

    public class NodeCrackCommand : NodeSubstateCommand
    {
        public override NodeSubstate SubstateType => NodeSubstate.Crack;
        public override void Execute()
        {
            
        }
    }

    public class NodeInfestCommand : NodeSubstateCommand
    {
        public override NodeSubstate SubstateType => NodeSubstate.Infest;
        protected override void InitHandler()
        {
            base.InitHandler();
            TileGameManager.I.TileSet.EventSystem.AddEvent(TurnEvent.TurnStart, OnTurnStart, Owner);
        }

        public override void Finish()
        {
            base.Finish();
            TileGameManager.I.TileSet.EventSystem.RemoveEvent(TurnEvent.TurnEnd, Execute, Owner);
        }

        private void OnTurnStart()
        {
            TileGameManager.I.TileSet.EventSystem.RemoveEvent(TurnEvent.TurnStart, OnTurnStart, Owner);
            TileGameManager.I.TileSet.EventSystem.AddEvent(TurnEvent.TurnEnd, Execute, Owner);
        }

        public override void Execute()
        {
            /*var infestTarget = TileGameManager.I.TileSet.GetTilesByRange(Owner.Coord, 1);
            var infestCoord = Random.Range(0, 8);

            int tempLoop = 0;
            int tempCoord=0;
            foreach (var tar in infestTarget)
            {
                if (tempLoop == 5)
                {
                    tempLoop++;
                    continue;
                }
                else
                {
                    tempLoop++;
                }
                if (tar.ValidNode() && tar != Owner && infestCoord == tempCoord)
                {
                    tar.SubstateSystem.SetState(NodeSubstate.Infest);
                }

                tempCoord++;
            }*/
            var infestTarget = TileGameManager.I.TileSet.GetNearTiles(Owner.Coord);
            infestTarget.RemoveAll(_ => _.SubstateSystem.Is(NodeSubstate.Infest));
            if (infestTarget.Count > 0)
            {
                var target = Random.Range(0, infestTarget.Count);
                infestTarget[target].SubstateSystem.SetState(NodeSubstate.Infest);
            }
        }
    }
}