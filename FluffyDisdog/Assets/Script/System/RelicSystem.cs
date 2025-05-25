using System.Collections.Generic;
using FluffyDisdog.Data.RelicData;
using Script.FluffyDisdog.Managers;

namespace FluffyDisdog
{
    public class RelicSystem
    {
        private PlayerManager player;

        
        /// <summary>
        /// 임시
        /// </summary>
        private RelicData[] datas;
        
        private List<RelicCommandData.RelicCommandData> commands;

        public RelicSystem(PlayerManager player)
        {
            this.player = player;
            commands = new List<RelicCommandData.RelicCommandData>();
        }

        private void InitRelic()
        {
            
        }

        public void InitNewCommand(RelicData data)
        {
            var newCommand = RelicCommandData.RelicCommandData.MakeRelicCommandData(data);
            commands.Add(newCommand);
        }
    }
}