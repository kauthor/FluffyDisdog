using System;
using UnityEngine;

namespace FluffyDisdog.Data.RelicData
{
    public enum RelicName
    {
        NONE=0,
        Oil=1
    }
    [Serializable]
    public class RelicData
    {
        public TurnEvent eventType;
        
        public RelicName relicName;

        [SerializeField] private float[] values;
        
        public float[] Values => values;

        public RelicData()
        {
            
        }
        
        #if UNITY_EDITOR
        public RelicData(TurnEvent newEvent, RelicName name, float[] val)
        {
            eventType = newEvent;
            relicName = name;
            values = val;
        }
        #endif
    }
}