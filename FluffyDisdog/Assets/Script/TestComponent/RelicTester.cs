using FluffyDisdog;
using FluffyDisdog.Data.RelicData;
using Script.FluffyDisdog.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FluffyDisdog
{
    public class RelicTester:MonoBehaviour
    {
        [SerializeField] private TurnEvent eventType;
        [SerializeField] private RelicName name;
        [SerializeField] private float[] values;

        [Button]
        private void MakeTestRelic()
        {
            RelicData data = new RelicData(eventType, name, values);
            PlayerManager.I.RelicSystem.InitNewCommand(data);
        }
    }
}