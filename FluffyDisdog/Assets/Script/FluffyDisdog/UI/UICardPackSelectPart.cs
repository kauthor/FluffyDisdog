using System;
using UnityEngine;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{
    public class UICardPackSelectPart:MonoBehaviour
    {
        [SerializeField] private Image cardPackImage;
        [SerializeField] private Button btnPackOpen;
        [SerializeField] private GameObject pnlPurchase;

        [SerializeField] private Text txtCardPackPrice;
        //[SerializeField] private Text txtRelicName;
        
        private event Action<int, int, UICardPackSelectPart> onCardPackOpen;
        private bool purchased=false;
        public bool Purchased => purchased;
        private void Start()
        {
            btnPackOpen.onClick.RemoveAllListeners();
            btnPackOpen.onClick.AddListener(OpenPack);
        }

        private int cost;
        private int gachaType;

        public void Init(int cost, int gachaType, Action<int, int, UICardPackSelectPart> onCardPackOpen)
        {
            this.cost = cost;
            this.gachaType = gachaType;
            this.onCardPackOpen -= onCardPackOpen;
            this.onCardPackOpen += onCardPackOpen;
            purchased=false;
            pnlPurchase.SetActive(false);
            txtCardPackPrice.text = cost.ToString();
        }

        public void Reroll(int cost, int gachaType, Action<int, int, UICardPackSelectPart> onCardPackOpen)
        {
            if (purchased)
                return;
            Init(cost, gachaType, onCardPackOpen);
        }
        
        
        private void OpenPack()
        {
            if (purchased)
                return;
            onCardPackOpen?.Invoke(cost,gachaType, this);
        }
        
        public void AfterBuy()
        {
            pnlPurchase.SetActive(true);
            purchased = true;
        }
        
    }
}