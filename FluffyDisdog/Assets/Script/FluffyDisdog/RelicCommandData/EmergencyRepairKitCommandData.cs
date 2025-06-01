using FluffyDisdog.Data.RelicData;
using Script.FluffyDisdog.Managers;

namespace FluffyDisdog.RelicCommandData
{
    public class ToolConsumeDesire : TurnEventOptionParam
    {
        //public float saveRate;
        public bool consumed = true;
    }
        
    public class EmergencyRepairKitCommandData:RelicCommandData
    {
        public override RelicName relicType => RelicName.EmergencyRepairKit;

        protected override void OnExecuteCommand(TurnEventOptionParam param)
        {
            base.OnExecuteCommand(param);
            if (param is ToolConsumeDesire des)
            {
                var seed = SeedManager.I.GetMinor();
                var rand = seed % 1000;
                if (rand / 1000.0f < rawData.Values[0])
                {
                    des.consumed = false;
                }
            }
            
        }

        public override void InitCommandData(RelicData data)
        {
            base.InitCommandData(data);
        }
    }
}
