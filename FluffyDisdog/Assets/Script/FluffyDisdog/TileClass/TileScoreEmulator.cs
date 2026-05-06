using System.Collections.Generic;
using FluffyDisdog;

namespace Script.FluffyDisdog.TileClass
{
    public class TileScoreEmulator
    {
        private Dictionary<int, float> tagMulti = new();
        private Dictionary<NodeSubstate, float> nodeSubMulti = new();

        public void SetTagScoreMulitplier(int tag, float value)
        {
            if(!tagMulti.TryAdd(tag, value))
                tagMulti[tag] += value;
        }
        
        public void SetNodeSubstateMulti(NodeSubstate sub, float value)
        {
            if(!nodeSubMulti.TryAdd(sub, value))
                nodeSubMulti[sub] += value;
        }
        
        public float GetTagMulti(int tag)
        => tagMulti.GetValueOrDefault(tag, 0);
        
        public float GetNodeSubstateMulti(NodeSubstate tag)
        => nodeSubMulti.GetValueOrDefault(tag, 0);
    }
}