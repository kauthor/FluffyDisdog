using FluffyDisdog.Data.RelicData;
using UnityEngine;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{
    public class UIRelicInfoPart:MonoBehaviour
    {
        [SerializeField] private Text txtRelicName;
        [SerializeField] private Image imgRelicIcon;

        private RelicName relicName;
        public RelicName RelicName => relicName;
        
        public void InitData(RelicName relic)
        {
            relicName = relic;
            txtRelicName.text = relicName.ToString();
        }

        public void ReturnToPool()
        {
            gameObject.SetActive(false);
        }
    }
}