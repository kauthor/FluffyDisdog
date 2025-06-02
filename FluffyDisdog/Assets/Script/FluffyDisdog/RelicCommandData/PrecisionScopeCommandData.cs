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
            if (param is CrackPointMeasureParam cparam)
            {
                var xdist = cparam.clicked.Item1 - cparam.target.Item1;
                xdist = Mathf.Abs(xdist);
                
                var ydist = cparam.clicked.Item2 - cparam.target.Item2;
                ydist = Mathf.Abs(ydist);
                
                cparam.addedRate += (xdist + ydist) * rawData.Values[0];
            }
        }

        public override void InitCommandData(RelicData data)
        {
            base.InitCommandData(data);
            PlayerManager.I.TurnEventSystem.AddEvent(TurnEvent.DistanceDesire, ExecuteCommand, this);
        }
    }
}
