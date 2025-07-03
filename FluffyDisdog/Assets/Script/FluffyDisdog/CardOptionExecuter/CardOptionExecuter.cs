using FluffyDisdog.Data;

namespace FluffyDisdog.CardOptionExecuter
{
    public class CardOptionExecuter:IEventAffectable
    {
        protected ToolCardOpData rawData;
        
        public virtual void InitCommandData(ToolCardOpData data)
        {
            rawData = data;
        }

        public void ExecuteCommand(TurnEventOptionParam param)
        {
            OnExecuteCommand(param);
        }

        protected virtual void OnExecuteCommand(TurnEventOptionParam param)
        {
            
        }
    }
}