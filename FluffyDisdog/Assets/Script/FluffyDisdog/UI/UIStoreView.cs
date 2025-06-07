using FluffyDisdog.Data.RelicData;
using Script.FluffyDisdog.Managers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{
    public class UIStoreView : UIViewBehaviour
    {
        public override UIType type => UIType.Store;

        [SerializeField] private Text txtGold;

        [SerializeField] private UICardPackSelectPart[] packs;
        [SerializeField] private UIRelicSelectPart[] relics;

        [FormerlySerializedAs("cardSlot")]
        [FoldoutGroup("Store")]
        [SerializeField] private CardPopupParts[] specialCardSlot;

        [FoldoutGroup("Store")] [SerializeField]
        private Button btnReroll;

        [FoldoutGroup("Store")] [SerializeField]
        private Button btnStoreAccept;

        [FoldoutGroup("Store")] [SerializeField]
        private GameObject pnlSoldOut;


        [SerializeField] private int storeValueEditor=20;

        private int currentAccountGold;

        private ToolType currentStoreSelected;

        public override void Init(UIViewParam param)
        {
            base.Init(param);
            
            SyncGold();
            
            foreach (var p in packs)
            {
                p.Init(Random.Range(20,31),0, OnClickGachaPack); //todo : 가챠타입, 비용 임시
                p.gameObject.SetActive(true);
            }

            foreach (var r in relics)
            {
                r.Init(20, (int)Random.Range(0, (int)RelicName.HorizontalVerticalStabilizer +1), OnClickRelicPack);
                r.gameObject.SetActive(true);
            }

            foreach (var c in specialCardSlot)
            {
                c.Init((ToolType)Random.Range(0, (int)ToolType.MAX),1);
                c.BindHandler(OnBuySpecialCard);
                c.gameObject.SetActive(true);
            }
            
            btnReroll.onClick.RemoveAllListeners();
            btnReroll.onClick.AddListener(Reroll);
        }


        private void Reroll()
        {
            foreach (var r in relics)
            {
                if(!r.Purchased)
                    r.Reroll(20, (int)Mathf.Max(0, (int)RelicName.HorizontalVerticalStabilizer +1), OnClickRelicPack);
            }

            foreach (var p in packs)
            {
                if(!p.Purchased)
                    p.Reroll(20,0, OnClickGachaPack);
            }
        }

        private void SyncGold()
        {
            currentAccountGold = AccountManager.I.Gold;
            txtGold.text = currentAccountGold.ToString();
        }

        private void OnClickGachaPack(int gachaType, int cost, UICardPackSelectPart slot)
        {
            if (cost > AccountManager.I.Gold)
                return;
            
            UICardPackResultPopup.OpenPopup(gachaType, cost);
            slot.AfterBuy();
        }
        
        private void OnClickRelicPack(int cost, RelicName relicName, UIRelicSelectPart slot)
        {
            if (cost > AccountManager.I.Gold)
                return;
            
            //UICardPackResultPopup.OpenPopup(gachaType, cost);
            AccountManager.I.GoldConsume(cost);
            TileGameManager.I.RelicSystem.GainRelic(relicName);
            
            slot.AfterBuy();
        }

        private void OnBuySpecialCard(ToolType tool, CardPopupParts slot)
        {
            if (AccountManager.I.Gold < 10) //todo : 임시비용 
                return;

            AccountManager.I.GoldConsume(10);
            slot.gameObject.SetActive(false);
            DeckManager.I.TryAddDeck(tool);
        }
        
    }
}