using FluffyDisdog.Data.RelicData;
using Script.FluffyDisdog.Managers;

namespace FluffyDisdog.RelicCommandData
{
    public abstract class RelicCommandData: IEventAffectable
    {
        protected RelicData rawData;

        public abstract RelicName relicType { get; }

        //protected PlayerManager player;
        public virtual void InitCommandData(RelicData data)
        {
            rawData = data;
        }

        protected void ExecuteCommand(TurnEventOptionParam param)
        {
            OnExecuteCommand(param);
        }

        protected virtual void OnExecuteCommand(TurnEventOptionParam param)
        {
            
        }
        
        public static RelicCommandData MakeRelicCommandData(RelicData rawData)
        {
            switch (rawData.relicName)
            {
                case RelicName.Oil:
                default:
                    var ret = new OilCommandData();
                    ret.InitCommandData(rawData);
                    return ret;
                    //return new OilCommandData();
            }
            return new OilCommandData();
        }
    }
}