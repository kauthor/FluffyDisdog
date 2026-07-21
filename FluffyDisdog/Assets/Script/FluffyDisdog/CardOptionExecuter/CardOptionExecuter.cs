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

        protected CardExecuteParam()
        {
            
        }
    }
    
    
    public class CardOptionExecuter:IEventAffectable
    {
        protected ToolCardOpData rawData;
        public virtual CardExecuteType ExecuteType => CardExecuteType.Pre;

        public static CardOptionExecuter MakeCardAddOptionExecuter(ToolCardOpData data)
        {
            CardOptionExecuter ret;
            switch (data.CardAddType)
            {
                case 1:
                    ret = new RandomExecuter();
                    break;
                case 2:
                    ret = new CrackExecuter();
                    break;
                case 3:
                    ret = new DarkLitExecuter();
                    break;
                case 4:
                    ret = new TreasureInteractExecuter();
                    break;
                case 5:
                    ret = new ObstacleInteractExecuter();
                    break;
                case 6:
                    ret = new CrackCrushExecuter();
                    break;
                case 7:
                    ret = new TreasureCrushExecuter();
                    break;
                case 8:
                    ret = new ObstacleCrushExecuter();
                    break;
                case 10:
                    ret = new CrushAllInteractedExecuter();
                    break;
                case 11:
                    ret = new DrawGoldCardExecuter();
                    break;
                case 12:
                    ret = new LineClearExecuter();
                    break;
                default:
                    ret = new CardOptionExecuter();
                    break;
            }

            ret.InitData(data);
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

        public void ExecuteWhenTileSuccess(CardExecuteParam param)
        {
            OnSuccess(param);
        }

        public void ExecuteWhenTileTryInteract(CardExecuteParam param)
        {
            OnTryInteract(param);
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

        protected virtual void OnSuccess(CardExecuteParam param)
        {
            
        }

        protected virtual void OnTryInteract(CardExecuteParam param)
        {
            
        }
    }
}