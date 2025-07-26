namespace FluffyDisdog.CardOptionExecuter
{
    public class CrackExecuter:CardOptionExecuter
    {
        protected override void OnExecuteTileEffect(CardExecuteParam param)
        {
            base.OnExecuteTileEffect(param);
            param.target.SubstateSystem.SetState(NodeSubstate.Crack);
        }
    }
}