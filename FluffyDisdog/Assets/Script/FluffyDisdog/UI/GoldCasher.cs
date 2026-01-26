using UnityEngine;

namespace FluffyDisdog.UI
{
    public class GoldCasher:MonoBehaviour
    {
        [SerializeField] private BinaryGoldPrefab[] goldText;
        
        public void SyncGold(int gold)
        {
            for (int i = 0; i < goldText.Length; i++)
            {
                goldText[i].SetBinary(gold%10);
                gold = gold/10;
            }
        }
    }
}