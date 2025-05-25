namespace FluffyDisdog.RelicCommandData
{
    public class OilCommandData:RelicCommandData
    {
        public override void ExecuteCommand(TurnEventOptionParam param)
        {
            if(rawData!=null && rawData.Values.Length >0)
               TileGameManager.I.AddScore((int)rawData.Values[0]);
        }
    }
}