using FluffyDisdog.Data.RelicData;
using FluffyDisdog.UI;
using Script.FluffyDisdog.Managers;
using UnityEngine;

namespace FluffyDisdog
{
    public class RequestSystem
    {
        private int reqStartLevel;
        private int reqEndLevel;
        private bool isReqRunning;
        private int reqRewardLevelAdd;
        private int reqDegree;

        public int ReqStartLevel => reqStartLevel;
        public int ReqEndLevel => reqEndLevel;
        public bool IsReqRunning => isReqRunning;
        public int ReqRewardLevelAdd => reqRewardLevelAdd;
        public int ReqDegree => reqDegree;
        
        public int DayFlow => TileGameManager.I.currentLevel - reqStartLevel;

        private int defaultSuccessRate=40;
        public int DefaultSuccessRate => defaultSuccessRate;
        
        public void Init()
        {
            reqStartLevel = 0;
            reqEndLevel = 0;
            isReqRunning = false;
            reqDegree = 0;
            reqRewardLevelAdd = 0;
        }

        public void StartRequest(int level, int degree)
        {
            reqStartLevel = level;
            isReqRunning = true;
            reqDegree = Mathf.Max(degree,1);
            reqRewardLevelAdd = 0;
        }
        

        public bool IsRequestReceivable()
        {
            return isReqRunning;
        }
        
        public void RewardLevelAdd() => reqRewardLevelAdd++;

        public bool RequestEnd(out int reward)
        {
            isReqRunning = false;
            int add = 0;
            reward = 0;
            if (PlayerManager.I.TurnEventSystem.HasRelicCommand(RelicName.TradersCompanion))
            {
                var data = PlayerManager.I.TurnEventSystem.GetRelicCommand(RelicName.TradersCompanion).RawData;
                add = (int)data.Values[0];
            }

            var reqData = ExcelManager.I.GetRequestData(reqDegree);
            var box = ExcelManager.I.GetRequestData(reqDegree).successBoxId;
            var failBox = ExcelManager.I.GetRequestData(reqDegree).failBoxId;
            if (failBox == 0)
                failBox = box;  //데이터 없을 때를 위한 예외처리
            var superBox = ExcelManager.I.GetRequestData(reqDegree).successBoxId; //잭팟이... 없다?
            
            
            var successRate = reqData.successRate + reqData.successRateInvest * reqRewardLevelAdd + 
                              reqData.successRatePerVisit * DayFlow;
            
            var jackpotRate = reqData.jackpotRate +
                              reqData.jackpotRateInvest * DayFlow;
            
            bool success = Random.Range(0,10000) < successRate;
            bool jackPot = Random.Range(0, 10000) < jackpotRate;

            var boxData = success == false
                ? ExcelManager.I.GetBoxItemData(failBox)
                : (jackPot ? ExcelManager.I.GetBoxItemData(superBox) : ExcelManager.I.GetBoxItemData(box));

            bool opengacha = false;
            foreach (var item in boxData)
            {
                switch (item.rewardType)
                {
                    case 1:
                        reward += item.rewardValue;
                        break;
                    case 6:
                        if (!opengacha)
                        {
                            opengacha = true;
                            
                            
                            UICardPackResultPopup.OpenPopup(item.rewardValue,0);
                        }

                        break;
                }
            }

            reqRewardLevelAdd = 0;
            reqDegree = 0;
            reqStartLevel = 0;
            
            return false;
        }
    }
}