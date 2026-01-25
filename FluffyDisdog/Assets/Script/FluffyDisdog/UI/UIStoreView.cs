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
        
        [FoldoutGroup("Store")]
        [SerializeField] private Text[] txtSpecialPrice;

        [FoldoutGroup("Store")] [SerializeField]
        private Button btnReroll;
        
        [SerializeField]
        private Button btnNextStage;

        [SerializeField] private int storeValueEditor=20;

        private int currentAccountGold;

        private ToolType currentStoreSelected;

        [FoldoutGroup("Request")] [SerializeField]
        private GameObject pnlRequestStart;

        [FoldoutGroup("Request")] [SerializeField]
        private GameObject pnlRequestStartAccept;
        
        [FoldoutGroup("Request")] [SerializeField]
        private GameObject pnlRequestAdd;

        [FoldoutGroup("Request")] [SerializeField]
        private GameObject pnlRequestAddComplete;
        
        [FoldoutGroup("Request")] [SerializeField]
        private Button[] btnRequestStart;

        [FoldoutGroup("Request")] [SerializeField]
        private Button btnRequestAdd;
        
        [FoldoutGroup("Request")] [SerializeField]
        private Button btnRequestComplete;

        private int requestAddPrice;

        [SerializeField]private Text txtRequestAddPrice;
        
        [SerializeField] private GoldCasher goldCasher;

        [SerializeField] private Button btnOption;

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
                r.Init(20, (int)Random.Range(101, (int)RelicName.HorizontalVerticalStabilizer +1), OnClickRelicPack);
                r.gameObject.SetActive(true);
            }


            int special = 0;
            foreach (var c in specialCardSlot)
            {
                c.Init((ToolType)Random.Range(0, (int)ToolType.MAX),0);
                c.BindHandler(OnBuySpecialCard);
                c.gameObject.SetActive(true);
                txtSpecialPrice[special++].text = $"10 G"; //todo 가격 임시
            }
            
            btnReroll.onClick.RemoveAllListeners();
            btnReroll.onClick.AddListener(Reroll);

            if (TileGameManager.I.RequestSystem.IsReqRunning)
            {
                pnlRequestStart.SetActive(false);
                pnlRequestAdd.SetActive(true);
                requestAddPrice = TileGameManager.I.RequestSystem.ReqDegree * 5;
                txtRequestAddPrice.text = requestAddPrice.ToString();
                txtRequestAddPrice.gameObject.SetActive(true);
            }
            else
            {
                pnlRequestStart.SetActive(true);
                pnlRequestAdd.SetActive(false);
                
                txtRequestAddPrice.gameObject.SetActive(false);
            }
            
            pnlRequestStartAccept.gameObject.SetActive(false);
            pnlRequestAddComplete.gameObject.SetActive(false);
            
            btnRequestStart[0].onClick.RemoveAllListeners();
            btnRequestStart[1].onClick.RemoveAllListeners();
            btnRequestStart[2].onClick.RemoveAllListeners();
            
            btnRequestStart[0].onClick.AddListener(()=>StartRequest(1));
            btnRequestStart[1].onClick.AddListener(()=>StartRequest(2));
            btnRequestStart[2].onClick.AddListener(()=>StartRequest(3));
            
            btnRequestAdd.onClick.RemoveAllListeners();
            btnRequestAdd.onClick.AddListener(RequestAdd);
            
            btnRequestComplete.onClick.RemoveAllListeners();
            btnRequestComplete.onClick.AddListener(RequestComplete);
            
            btnNextStage.onClick.RemoveAllListeners();
            btnNextStage.onClick.AddListener(() =>
            {
                TileGameManager.I.GoNextLevel();
            });
            
            btnOption.onClick.RemoveAllListeners();
            btnOption.onClick.AddListener(() =>
            {
                UIOptionPopup.OpenPopup();
            });
        }

        private void StartRequest(int deg)
        {
            TileGameManager.I.RequestSystem.StartRequest(TileGameManager.I.currentLevel,deg);
            AccountManager.I.GoldConsume(deg*5);
            pnlRequestStartAccept.gameObject.SetActive(true);
            
            SyncGold();
        }

        private void RequestAdd()
        {
            TileGameManager.I.RequestSystem.RewardLevelAdd();
            AccountManager.I.GoldConsume(requestAddPrice);
            pnlRequestAddComplete.gameObject.SetActive(true);
            
            SyncGold();
        }

        private void RequestComplete()
        {
            pnlRequestAddComplete.gameObject.SetActive(true);
            TileGameManager.I.RequestSystem.RequestEnd(out var ret);
            AccountManager.I.AddGold(ret);
            
            SyncGold();
        }


        private void Reroll()
        {
            foreach (var r in relics)
            {
                if(!r.Purchased)
                    r.Reroll(20, (int)Random.Range(1, (int)RelicName.HorizontalVerticalStabilizer +1), OnClickRelicPack);
            }

            foreach (var p in packs)
            {
                if(!p.Purchased)
                    p.Reroll(Random.Range(20,31),0, OnClickGachaPack);
            }
            AccountManager.I.GoldConsume(5);
            SyncGold();
        }

        private void SyncGold()
        {
            currentAccountGold = AccountManager.I.Gold;
            //txtGold.text = currentAccountGold.ToString();
            goldCasher.SyncGold(currentAccountGold);
        }

        private void OnClickGachaPack(int cost, int gachaType, UICardPackSelectPart slot)
        {
            if (cost > AccountManager.I.Gold)
                return;

            AccountManager.I.GoldConsume(cost);
            SyncGold();
            UICardPackResultPopup.OpenPopup(gachaType, cost);
            slot.AfterBuy();
        }
        
        private void OnClickRelicPack(int cost, RelicName relicName, UIRelicSelectPart slot)
        {
            if (cost > AccountManager.I.Gold)
                return;
            
            //UICardPackResultPopup.OpenPopup(gachaType, cost);
            AccountManager.I.GoldConsume(cost);
            SyncGold();
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