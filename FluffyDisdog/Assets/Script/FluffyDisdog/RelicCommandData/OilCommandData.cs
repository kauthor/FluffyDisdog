namespace FluffyDisdog.RelicCommandData
{
    public class OilCommandData:RelicCommandData
    {
        protected override void OnExecuteCommand (TurnEventOptionParam param)
        {
            if(rawData!=null && rawData.Values.Length >0)
               TileGameManager.I.AddScore((int)rawData.Values[0]);
        }
    }
}