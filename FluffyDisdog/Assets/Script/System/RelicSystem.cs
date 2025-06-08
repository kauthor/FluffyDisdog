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

        public RelicData[] currentRelicDatas => datas;
        
        private List<RelicCommandData.RelicCommandData> commands;

        private List<RelicName> relicList;

        public RelicSystem(PlayerManager player)
        {
            this.player = player;
            commands = new List<RelicCommandData.RelicCommandData>();
            relicList = new List<RelicName>();
        }

        public void GainRelic(RelicName relic)
        {
            relicList.Add(relic);
        }

        public void LoseRelic(RelicName relic)
        {
            if(relicList.Contains(relic))
               relicList.Remove(relic);
        }

        public void InitStageRelic()
        {
            commands.Clear();
            if (relicList.Count <= 0)
                return;
            
            
            datas = new RelicData[relicList.Count];
            for (int i = 0; i < relicList.Count; i++)
            {
                datas[i] = ExcelManager.I.GetRelicData(relicList[i]);
                InitNewCommand(datas[i]);
            }
        }

        public void InitNewCommand(RelicData data)
        {
            var newCommand = RelicCommandData.RelicCommandData.MakeRelicCommandData(data);
            commands.Add(newCommand);
        }
    }
}