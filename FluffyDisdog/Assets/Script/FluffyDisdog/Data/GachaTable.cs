using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FluffyDisdog.Data
{
    [Serializable]
    public class GachaData
    {
        public int id;
        public int gachaType;
        public int gachaId;
        public int rate;
        public int rewardType;
        public string rewardValue;
        public int rewardCount;
    }

    public class GacheInGameData
    {
        public int gachaType;
        public int gachaId;
        public int rateSum;
        public int[] rateArray;
        public int[] key;
    }
    
    [CreateAssetMenu]
    public class GachaTable:ScriptableObject
    {
        [SerializeField] private GachaData[] datas;
        
#if UNITY_EDITOR
        public void SetData(GachaData[] d)
        {
            datas = d;
        }
#endif

        public Dictionary<int, GacheInGameData> TryCache()
        {
            var ret = new Dictionary<int, GacheInGameData>();

            for (int i = 0; i < datas.Length; )
            {
                var curdata = datas[i];
                GacheInGameData targetDic;
                if (ret.ContainsKey(curdata.gachaId))
                {
                    i++;
                    continue;
                }
                else
                {
                    int startInd = i;
                    int count = datas.Count(_ => _.gachaId == curdata.gachaId);
                    int endInd = i+ count-1;
                    int sum = 0;
                    
                    int[] valueArr = new int[count];
                    int[] keyArr = new int[count];
                    for (int j = 0; j < count; j++)
                    {
                        sum += datas[j+startInd].rate;
                        valueArr[j] = sum;
                        keyArr[j] = curdata.id-1;
                    }

                    GacheInGameData target = new GacheInGameData()
                    {
                        gachaId = curdata.gachaId,
                        gachaType = curdata.gachaType,
                        rateSum = sum,
                        rateArray = valueArr,
                        key = keyArr,
                    };
                    ret.Add(curdata.gachaId, target);
                    
                    i=endInd+1;
                }
            }
            
            return ret;
        }
    }
}