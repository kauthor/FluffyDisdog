using System;
using FluffyDisdog.Data.RelicData;
using Script.FluffyDisdog.Managers;

namespace FluffyDisdog.RelicCommandData
{
    public class CrackPointMeasureParam:TurnEventOptionParam
    {
        //public bool horizon;
        //public bool vertical;
        public Tuple<int,int> clicked;
        public Tuple<int,int> target;
        public float addedRate;
    }
    public class HorizontalVerticalStabilizerCommandData:RelicCommandData
    {
        public override RelicName relicType => RelicName.HorizontalVerticalStabilizer;

        protected override void OnExecuteCommand(TurnEventOptionParam param)
        {
            base.OnExecuteCommand(param);
            if (param is CrackPointMeasureParam horver)
            {
                if(horver.clicked.Item1 == horver.target.Item1
                   || horver.clicked.Item2 == horver.target.Item2)
                horver.addedRate = rawData.Values[0];
            }
        }

        public override void InitCommandData(RelicData data)
        {
            base.InitCommandData(data);
            PlayerManager.I.TurnEventSystem.AddEvent(TurnEvent.HorOrVerTileTry, ExecuteCommand, this);
        }
    }
}
