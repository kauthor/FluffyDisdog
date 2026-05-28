using FluffyDisdog.Data.RelicData;
using Script.FluffyDisdog.Managers;
using UnityEngine;

namespace FluffyDisdog.RelicCommandData
{
    public class SmallGoldIngotCommandData:RelicCommandData
    {
        public override RelicName relicType => RelicName.SmallGoldIngot;

        public override void InitCommandData(RelicData data)
        {
            base.InitCommandData(data);
            PlayerManager.I.TurnEventSystem.AddEvent(TurnEvent.GameStart, ExecuteCommand, this);
        }

        protected override void OnExecuteCommand(TurnEventOptionParam param)
        {
            base.OnExecuteCommand(param);
            var currentGold = Mathf.Min(rawData.Values[2], AccountManager.I.Gold);
            
            var mult = ((int)currentGold )/ (int)rawData.Values[0];
            TileGameManager.I.AddScore(mult*(int)rawData.Values[1]);
        }
    }
}