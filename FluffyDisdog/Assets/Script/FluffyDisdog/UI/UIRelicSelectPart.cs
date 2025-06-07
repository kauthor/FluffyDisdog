using System;
using FluffyDisdog.Data.RelicData;
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
            this.onCardPackOpen -= onCardPackOpen;
            this.onCardPackOpen += onCardPackOpen;
            pnlPurchase.SetActive(false);
            purchased = false;
            txtRelicName.text = relicType.ToString();
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