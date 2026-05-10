using System;
using System.Collections.Generic;
using UnityEngine;

namespace FluffyDisdog.Data
{
    [Serializable]
    public class RequestData
    {
        public int id;
        public int requestGrade;
        public int cost;
        public int maxInvest;
        public int successRate;
        public int successRatePerVisit;
        public int successRateInvest;
        public int jackpotRate;
        public int jackpotRateInvest;
        public int failBoxId;
        public int successBoxId;
    }
    public class RequestTable:ScriptableObject
    {
        [SerializeField] private RequestData[] datas;
#if UNITY_EDITOR
        public void SetData(RequestData[] d)
        {
            datas = d;
        }
#endif
        
        public Dictionary<int, RequestData> TryCache()
        {
            Dictionary<int, RequestData> ret = new Dictionary<int, RequestData>();
            foreach (var d in datas)
                ret.Add(d.id,d);
            
            return ret;
        }
    }
}