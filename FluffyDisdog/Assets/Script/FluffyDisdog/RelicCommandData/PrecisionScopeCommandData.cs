using FluffyDisdog.Data.RelicData;
using Script.FluffyDisdog.Managers;
using UnityEngine;

namespace FluffyDisdog.RelicCommandData
{
    public class PrecisionScopeCommandData : RelicCommandData
    {
        public override RelicName relicType => RelicName.PrecisionScope;

        protected override void OnExecuteCommand(TurnEventOptionParam param)
        {
            base.OnExecuteCommand(param);
            if (param is TileEmulatorOptionParam cparam)
            {
                var xdist = cparam.clickedCoord.Item1 - cparam.targetCoord.Item1;
                xdist = Mathf.Abs(xdist);
                
                var ydist = cparam.clickedCoord.Item2 - cparam.targetCoord.Item2;
                ydist = Mathf.Abs(ydist);
                
                int dist = ydist + xdist;
                if (dist > 0 && dist <= rawData.Values.Length)
                {
                    cparam.addToolRate += rawData.Values[dist - 1] / 10000.0f;
                }
            }
        }

        public override void InitCommandData(RelicData data)
        {
            base.InitCommandData(data);
            PlayerManager.I.TurnEventSystem.AddEvent(TurnEvent.DistanceDesire, ExecuteCommand, this);
        }
    }
}
