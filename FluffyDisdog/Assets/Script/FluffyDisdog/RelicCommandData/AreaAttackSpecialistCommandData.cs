using FluffyDisdog.Data.RelicData;
using Script.FluffyDisdog.Managers;

namespace FluffyDisdog.RelicCommandData
{
    public class OnEndCrackParam : TurnEventOptionParam
    {
        public int digged;
        public float addedScoreRate;
    }
    public class AreaAttackSpecialistCommandData:RelicCommandData
    {
        public override RelicName relicType => RelicName.AreaAttackSpecialist;

        override public void InitCommandData(RelicData data)
        {
            base.InitCommandData(data);
            PlayerManager.I.TurnEventSystem.AddEvent(TurnEvent.EndCrack, ExecuteCommand, this);
        }

        protected override void OnExecuteCommand(TurnEventOptionParam param)
        {
            base.OnExecuteCommand(param);

            if (param is OnEndCrackParam eparam)
            {
                if (eparam.digged >= rawData.Values[0])
                {
                    eparam.addedScoreRate += rawData.Values[1];
                }
            }
        }
    }
}
