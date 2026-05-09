using System;
using FluffyDisdog.Data.RelicData;
using Script.FluffyDisdog.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{
    public class UIRelicSelectPart:MonoBehaviour
    {
        [SerializeField] private Image relicImage;
        [SerializeField] private Button btnRelicBuy;
        [SerializeField] private GameObject pnlPurchase;
        
        [SerializeField] private Text txtRelicPrice;
        [SerializeField] private Text txtRelicName;
        [SerializeField] private Text txtRelicNameShadow;

        [SerializeField] private TextMeshProUGUI txtRelicDesc;

        private event Action<int, RelicName, UIRelicSelectPart> onCardPackOpen;

        private void Start()
        {
            btnRelicBuy.onClick.RemoveAllListeners();
            btnRelicBuy.onClick.AddListener(TryClickRelic);
        }

        private int cost;
        private RelicName relicType;
        private bool purchased=false;
        public bool Purchased => purchased;

        public void Init(int cost, int relicN, Action<int, RelicName,UIRelicSelectPart> onCardPackOpen)
        {
            this.cost = cost;
            this.relicType = (RelicName)relicN;
             ResourceLoadManager.I.LoadSpriteAtlasResource(ResourceAddress.RelicIcon,
                $"Relic{relicN}",
                _ =>
                {
                    if (_ != null)
                        relicImage.sprite = _;
                }).Forget();
            this.onCardPackOpen -= onCardPackOpen;
            this.onCardPackOpen += onCardPackOpen;
            pnlPurchase.SetActive(false);
            purchased = false;
            
            var data = ExcelManager.I.GetRelicData(relicType);
            var nameTxt = ExcelManager.I.GetLocalizeData(data.localKey).kor;
            var descTxt = ExcelManager.I.GetLocalizeData(data.localDesc).kor;
            
            txtRelicName.text = nameTxt;
            txtRelicDesc.SetText(descTxt);
            txtRelicPrice.text = cost.ToString();
        }

        public void Reroll(int cost, int relicN, Action<int, RelicName, UIRelicSelectPart> onCardPackOpen)
        {
            if (purchased)
                return;
            
            Init(cost, relicN, onCardPackOpen);
        }
        
        
        private void TryClickRelic()
        {
            if (purchased)
                return;
            onCardPackOpen?.Invoke(cost,relicType,this);
        }

        public void AfterBuy()
        {
            pnlPurchase.SetActive(true);
            purchased = true;
        }
    }
}