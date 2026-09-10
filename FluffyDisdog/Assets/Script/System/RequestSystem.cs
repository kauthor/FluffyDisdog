using System.Collections.Generic;
using System.Data;
using System.Linq;
using FluffyDisdog.Data;
using FluffyDisdog.Data.RelicData;
using FluffyDisdog.UI;
using Script.FluffyDisdog.Managers;
using UnityEngine;

namespace FluffyDisdog
{
    public enum RequestRewardType
    {
        NONE=0,
        Gold=1,
        Card,
        Relic,
        Pack,
        Weight,
        Gacha,
        RemoveOrUpgrade
    }
    public class RequestReward
    {
        public RequestRewardType Type;
        public int value;
        public int count;
    }
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
            var superBox = ExcelManager.I.GetRequestData(reqDegree).jackpotBoxId; //잭팟이... 없다?
            
            
            var successRate = reqData.successRate + reqData.successRateInvest * reqRewardLevelAdd + 
                              reqData.successRatePerVisit * DayFlow;
            
            var jackpotRate = reqData.jackpotRate +
                              reqData.jackpotRateInvest * DayFlow;


            int gacha = Random.Range(0, 10000);
            bool success = gacha < successRate+jackpotRate;
            bool jackPot = gacha < jackpotRate;

            var boxItemData = success == false
                ? ExcelManager.I.GetBoxItemData(failBox)
                : (jackPot ? ExcelManager.I.GetBoxItemData(superBox) : ExcelManager.I.GetBoxItemData(box));
            
            var boxData = success == false
                ? ExcelManager.I.GetBoxData(failBox)
                : (jackPot ? ExcelManager.I.GetBoxData(superBox) : ExcelManager.I.GetBoxData(box));

            bool opengacha = false;
            
            /*foreach (var item in boxItemData)
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
            }*/

            int boxPickValue = boxData.pickCount;
            
            ExecuteReward(boxItemData, boxPickValue);

            reqRewardLevelAdd = 0;
            reqDegree = 0;
            reqStartLevel = 0;
            
            return false;
        }


        private void ExecuteReward(BoxItemData[] data, int count)
        {
            var dataList = new List<RequestReward>();
            if (count == 0)
            {
                foreach (var item in data)
                {
                    RequestReward reward = new RequestReward();
                    if (item.rewardType == 5)
                    {
                        var groupId = item.rewardValue;
                        var pool = ExcelManager.I.GetRequestWeightData(groupId);
                        var gachaPool = new Dictionary<int, int>();
                        int max = 0;
                        foreach (var card in DeckManager.I.GetDeckList())
                        {
                            if (pool.TryGetValue(card.ToolType.ToString(), out var value))
                            {
                                if(gachaPool.ContainsKey(value.gachaId))
                                    gachaPool[value.gachaId] += value.rate;
                                else
                                {
                                    gachaPool.Add(value.gachaId, value.rate);
                                }
                                max += value.rate;
                            }
                        }
                        var rand = Random.Range(0, max);
                        foreach (var pair in gachaPool)
                        {
                            rand -= pair.Value;
                            if (rand < 0)
                            {
                                reward.Type = RequestRewardType.Gacha;
                                reward.value = pair.Key;
                                dataList.Add(reward);
                            }
                        }
                    }
                    else
                    {
                        reward.Type = (RequestRewardType)item.rewardType;
                        reward.value = item.rewardValue;
                        reward.count = item.rewardCount;
                        dataList.Add(reward);
                    }
                }
            }
            else
            {
                var rand = data.OrderBy(x => Random.Range(0, 100));
                int temp = 0;
                foreach (var item in rand)
                {
                    RequestReward reward = new RequestReward();
                    if (item.rewardType == 5)
                    {
                        var groupId = item.rewardValue;
                        var pool = ExcelManager.I.GetRequestWeightData(groupId);
                        var gachaPool = new Dictionary<int, int>();
                        int max = 0;
                        foreach (var card in DeckManager.I.GetDeckList())
                        {
                            if (pool.TryGetValue(card.ToolType.ToString(), out var value))
                            {
                                if(gachaPool.ContainsKey(value.gachaId))
                                    gachaPool[value.gachaId] += value.rate;
                                else
                                {
                                    gachaPool.Add(value.gachaId, value.rate);
                                }
                                max += value.rate;
                            }
                        }
                        var r = Random.Range(0, max);
                        foreach (var pair in gachaPool)
                        {
                            r -= pair.Value;
                            if (r < 0)
                            {
                                reward.Type = RequestRewardType.Gacha;
                                reward.value = pair.Key;
                                dataList.Add(reward);
                            }
                        }
                    }
                    else
                    {
                        reward.Type = (RequestRewardType)item.rewardType;
                        reward.value = item.rewardValue;
                        reward.count = item.rewardCount;
                        dataList.Add(reward);
                    }
                    temp++;
                    if(temp >= count)
                        break;
                }
            }
            
            //todo::DataList 로 신규  UI구성
            UiRequestResultPopup.OpenPopup(dataList);
        }
    }
}