using FluffyDisdog.Data.RelicData;
using Script.FluffyDisdog.Managers;
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
        
        public void InitData(RelicName relic, string key="")
        {
            relicName = relic;
            //txtRelicName.text = relicName.ToString();
            txtRelicName.gameObject.SetActive(false);
            
            ResourceLoadManager.I.LoadRelicIcon($"Relic{(int)relicName}", _ =>
            {
                if (_ != null)
                    imgRelicIcon.sprite = _;
            }).Forget();
        }

        public void ReturnToPool()
        {
            gameObject.SetActive(false);
        }
    }
}