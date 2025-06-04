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
            var suc = SeedManager.I.GetStoreSeed() % (100);
            if (suc <= defaultSuccessRate + ReqRewardLevelAdd)
            {
                reward = 20 + reqDegree * 5; 
                AccountManager.I.AddGold(reward); //todo:임시
                return true;
            }

            reward = 0;
            return false;
        }
    }
}