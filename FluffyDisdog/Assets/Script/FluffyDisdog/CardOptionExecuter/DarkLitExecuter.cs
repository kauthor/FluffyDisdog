namespace FluffyDisdog.CardOptionExecuter
{
    public class DarkLitExecuter:CardOptionExecuter
    {
        protected override void OnExecuteTileEffect(CardExecuteParam param)
        {
            base.OnExecuteTileEffect(param);
            if(param.target.SubstateSystem.Is(NodeSubstate.Dark))
               param.target.SubstateSystem.RemoveState(NodeSubstate.Dark);
        }
    }
}