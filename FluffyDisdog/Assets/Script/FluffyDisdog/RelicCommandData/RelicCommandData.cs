using FluffyDisdog.Data.RelicData;
using Script.FluffyDisdog.Managers;

namespace FluffyDisdog.RelicCommandData
{
    public class RelicCommandData: IEventAffectable
    {
        protected RelicData rawData;
        //protected PlayerManager player;
        public virtual void InitCommandData(RelicData data)
        {
            rawData = data;
            PlayerManager.I.TurnEventSystem.AddEvent(rawData.eventType, ExecuteCommand, this);
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
                    var ret = new OilCommandData();
                    ret.InitCommandData(rawData);
                    return ret;
                    //return new OilCommandData();
                default:
                    return new RelicCommandData();
            }
            return new RelicCommandData();
        }
    }
}