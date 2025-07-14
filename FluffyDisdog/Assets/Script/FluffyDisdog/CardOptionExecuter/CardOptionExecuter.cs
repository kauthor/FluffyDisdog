using FluffyDisdog.Data;

namespace FluffyDisdog.CardOptionExecuter
{
    public enum CardExecuteType
    {
        Pre=0,
        OnTile=1,
        Post=2
    }

    public class CardExecuteParam
    {
        public TerrainNode target;
        public float input;
        public float output;

        public CardExecuteParam(TerrainNode target, float input)
        {
            this.target = target;
            this.input = input;
        }
    }
    
    
    public class CardOptionExecuter:IEventAffectable
    {
        protected ToolCardOpData rawData;
        public virtual CardExecuteType ExecuteType => CardExecuteType.Pre;

        public static CardOptionExecuter MakeCardAddOptionExecuter(ToolCardOpData data)
        {
            CardOptionExecuter ret;
            switch (data.Id)
            {
                case 1:
                default:
                    ret = new CardOptionExecuter().InitData(data);
                    break;
            }

            return ret;
        }
        
        public virtual CardOptionExecuter InitData(ToolCardOpData data)
        {
            rawData = data;
            return this;
        }

        public void PreEffect(CardExecuteParam param)
        {
            OnPreEffect(param);
        }

        public void ExecuteTileEffect(CardExecuteParam param)
        {
            OnExecuteTileEffect(param);
        }

        public void PostEffect(CardExecuteParam param)
        {
            OnPostEffect(param);
        }

        protected virtual void OnExecuteTileEffect(CardExecuteParam param)
        {
            
        }

        protected virtual void OnPreEffect(CardExecuteParam param)
        {
            
        }

        protected virtual void OnPostEffect(CardExecuteParam param)
        {
            
        }
    }
}