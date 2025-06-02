using FluffyDisdog.Data.RelicData;
using Script.FluffyDisdog.Managers;

namespace FluffyDisdog.RelicCommandData
{
    public class TileDiggedParam : TurnEventOptionParam
    {
        public TerrainNode target;
    }
    public class OilCommandData:RelicCommandData
    {
        public override RelicName relicType => RelicName.Oil;

        override public void InitCommandData(RelicData data)
        {
            base.InitCommandData(data);
            PlayerManager.I.TurnEventSystem.AddEvent(TurnEvent.TileDigged, ExecuteCommand, this);
        }
        protected override void OnExecuteCommand (TurnEventOptionParam param)
        {
            if(rawData!=null && rawData.Values.Length >0)
               TileGameManager.I.AddScore((int)rawData.Values[0]);
        }
    }
}