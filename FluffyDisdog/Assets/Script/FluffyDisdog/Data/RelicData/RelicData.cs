using UnityEngine;

namespace FluffyDisdog.Data.RelicData
{
    public enum RelicName
    {
        NONE=0,
        Oil=1
    }
    public class RelicData
    {
        public TurnEvent eventType;
        
        public RelicName relicName;

        [SerializeField] private float[] values;
        
        public float[] Values => values;
    }
}