using System.Collections.Generic;
using System.Linq;
using FluffyDisdog.Data.RelicData;
using Script.FluffyDisdog.Managers;
using Sirenix.OdinInspector;
using TMPro;
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

        [SerializeField] private Button btnShowDeck;

        [FormerlySerializedAs("cardSlot")]
        [FoldoutGroup("Store")]
        [SerializeField] private CardPopupParts[] specialCardSlot;
        
        [FoldoutGroup("Store")]
        [SerializeField] private OutlinedText[] txtSpecialPrice;

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
        private GameObject pnlRequestReceiveComplete;
        
        [FoldoutGroup("Request")] [SerializeField]
        private Button[] btnRequestStart;

        [FoldoutGroup("Request")] [SerializeField]
        private Button btnRequestAdd;
        
        [FoldoutGroup("Request")] [SerializeField]
        private Button btnRequestComplete;
        
        [FoldoutGroup("Request")] [SerializeField]
        private GameObject[] requestCheckImg;
        
        [FoldoutGroup("Request")] [SerializeField]
        private Text requestDayFlow;
        
        [FoldoutGroup("Request")] [SerializeField]
        private GameObject requestAddDisableBlur;

        private int requestAddPrice;

        [SerializeField]private Text txtRequestAddPrice;
        
        [SerializeField] private GoldCasher goldCasher;

        [SerializeField] private Button btnOption;


        [SerializeField] private Color disableCOlor;
        private int limit1Cost = 10;
        private int limit2Cost = 10;

        public override void Init(UIViewParam param)
        {
            base.Init(param);
            
            SyncGold();
            
            SoundManager.I.PlayBgm(SoundDesc.StoreBgm);
            SoundManager.I.StopEnv();
            
            btnShowDeck.onClick.RemoveAllListeners();
            btnShowDeck.onClick.AddListener(() =>
            {
                UIDeckListPopup.OpenPopup();
            });
            
            requestDayFlow.text = ($"DAY - {TileGameManager.I.RequestSystem.ReqRewardLevelAdd+1}");

            List<int> usedRelic=new List<int>();
            var curRelic = TileGameManager.I.RelicSystem.currentRelicDatas;
            if(curRelic != null)
                for (int i = 0; i < curRelic.Length; i++)
                {
                    usedRelic.Add((int)curRelic[i].relicName);
                }
            
            var pack1 = ExcelManager.I.ExecuteGacha(ExcelManager.I.GetShopData(1).gachaId);
            var pack2 = ExcelManager.I.ExecuteGacha(ExcelManager.I.GetShopData(2).gachaId);
            var pack3 = ExcelManager.I.ExecuteGacha(ExcelManager.I.GetShopData(3).gachaId);
            var limit1 = ExcelManager.I.ExecuteGacha(ExcelManager.I.GetShopData(6).gachaId);
            var limit2 = ExcelManager.I.ExecuteGacha(ExcelManager.I.GetShopData(7).gachaId);
            //var relic1 = ExcelManager.I.ExecuteGacha(ExcelManager.I.GetShopData(6).gachaId);
            string relic1Key = ExcelManager.I.ExecuteGacha(ExcelManager.I.GetShopData(4).gachaId).rewardValue;
            RelicName relic1Name = relic1Key
                .StringToRelicName();
            while (usedRelic.Contains((int)relic1Name) && usedRelic.Count <9 )
            {
                relic1Key = ExcelManager.I.ExecuteGacha(ExcelManager.I.GetShopData(4).gachaId).rewardValue;
                relic1Name = relic1Key
                    .StringToRelicName();
            }

            bool disableRelic1 = usedRelic.Count >= 9;
            usedRelic.Add((int)relic1Name);
            
            string relic2Key = ExcelManager.I.ExecuteGacha(ExcelManager.I.GetShopData(5).gachaId).rewardValue;
            RelicName relic2Name = relic2Key
                .StringToRelicName();
            while (usedRelic.Contains((int)relic2Name) && usedRelic.Count <9 )
            {
                relic2Key = ExcelManager.I.ExecuteGacha(ExcelManager.I.GetShopData(5).gachaId).rewardValue;
                relic2Name = relic2Key
                    .StringToRelicName();
            }
            bool disableRelic2 = usedRelic.Count >= 9;
            
            /*foreach (var p in packs)
            {
                p.Init(Random.Range(20,31),0, OnClickGachaPack); //todo : 가챠타입, 비용 임시
                p.gameObject.SetActive(true);
            }*/
            
            packs[0].Init(ExcelManager.I.GetShopCost(pack1.rewardValue),ExcelManager.I.GetPackData(pack1.rewardValue).gachaKey, OnClickGachaPack);
            packs[0].SetPackResource(ExcelManager.I.GetPackData(pack1.rewardValue).gachaKey-100);
            packs[0].gameObject.SetActive(true);
            
            packs[1].Init(ExcelManager.I.GetShopCost(pack2.rewardValue),ExcelManager.I.GetPackData(pack2.rewardValue).gachaKey, OnClickGachaPack);
            packs[1].SetPackResource(ExcelManager.I.GetPackData(pack2.rewardValue).gachaKey-100);
            packs[1].gameObject.SetActive(true);
            
            packs[2].Init(ExcelManager.I.GetShopCost(pack3.rewardValue),ExcelManager.I.GetPackData(pack3.rewardValue).gachaKey, OnClickGachaPack);
            packs[2].SetPackResource(ExcelManager.I.GetPackData(pack3.rewardValue).gachaKey-100);
            packs[2].gameObject.SetActive(true);
            
            /*foreach (var r in relics)
            {
                int rel = -1;
                if (usedRelic.Count >= 22)
                {
                    r.gameObject.SetActive(false);
                    continue;
                }
                for (;usedRelic.Contains(rel)||rel<0;)
                {
                    rel = Random.Range(101, (int)RelicName.HorizontalVerticalStabilizer + 1);
                }
                usedRelic.Add(rel);
                 
                r.Init(20, rel, OnClickRelicPack);
                r.gameObject.SetActive(true);
            }*/

            if (!disableRelic1)
            {
                relics[0].gameObject.SetActive(true);
                relics[0].Init(ExcelManager.I.GetShopCost(relic1Key), (int)relic1Name, OnClickRelicPack);
            }
            else
            {
                relics[0].gameObject.SetActive(false);
            }
            
            if (!disableRelic2)
            {
                relics[1].gameObject.SetActive(true);
                relics[1].Init(ExcelManager.I.GetShopCost(relic2Key), (int)relic2Name, OnClickRelicPack);
            }
            else
            {
                relics[1].gameObject.SetActive(false);
            }


            /*int special = 0;
            foreach (var c in specialCardSlot)
            {
                c.Init((ToolType)Random.Range(0, (int)ToolType.MAX),0);
                c.BindHandler(OnBuySpecialCard);
                c.gameObject.SetActive(true);
                txtSpecialPrice[special++].SetText($"10 G"); //todo 가격 임시
            }*/

            ToolType limit1Tool = (ToolType)System.Enum.Parse(typeof(ToolType), limit1.rewardValue);
            ToolType limit2Tool = (ToolType)System.Enum.Parse(typeof(ToolType), limit2.rewardValue);
            
            specialCardSlot[0].Init(limit1Tool, 0);
            specialCardSlot[0].BindHandler(OnBuySpecialCard);
            specialCardSlot[0].gameObject.SetActive(true);
            limit1Cost = ExcelManager.I.GetShopCost(limit1.rewardValue);
            txtSpecialPrice[0].SetText($"{limit1Cost} G");
            
            specialCardSlot[1].Init(limit2Tool, 0);
            specialCardSlot[1].BindHandler(OnBuySpecialCard);
            specialCardSlot[1].gameObject.SetActive(true);
            limit2Cost = ExcelManager.I.GetShopCost(limit2.rewardValue);
            txtSpecialPrice[1].SetText($"{limit2Cost} G");
            
            btnReroll.onClick.RemoveAllListeners();
            btnReroll.onClick.AddListener(Reroll);

            if (TileGameManager.I.RequestSystem.IsReqRunning)
            {
                pnlRequestStart.SetActive(false);
                
                pnlRequestAdd.SetActive(true);

                for (int i = 0; i < requestCheckImg.Length; i++)
                {
                    requestCheckImg[i].SetActive(i <= TileGameManager.I.RequestSystem.ReqRewardLevelAdd);
                }

                requestAddPrice = ExcelManager.I.GetRequestData(TileGameManager.I.RequestSystem.ReqDegree).cost;
                txtRequestAddPrice.text = $"Pay More {requestAddPrice} G";
                txtRequestAddPrice.gameObject.SetActive(true);
                btnRequestAdd.interactable = TileGameManager.I.RequestSystem.ReqRewardLevelAdd<3;
                txtRequestAddPrice.color = TileGameManager.I.RequestSystem.ReqRewardLevelAdd<3 ? Color.white : disableCOlor;
            }
            else
            {
                pnlRequestStart.SetActive(true);
                pnlRequestAdd.SetActive(false);
                
                txtRequestAddPrice.gameObject.SetActive(false);
            }
            
            
            pnlRequestStartAccept.gameObject.SetActive(false);
            pnlRequestAddComplete.gameObject.SetActive(false);
            pnlRequestReceiveComplete.gameObject.SetActive(false);
            
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
            var cost = ExcelManager.I.GetRequestData(deg).cost;
            if (AccountManager.I.Gold < cost)
                return;
            TileGameManager.I.RequestSystem.StartRequest(TileGameManager.I.currentLevel,deg);
            
            AccountManager.I.GoldConsume(cost);
            pnlRequestStartAccept.gameObject.SetActive(true);
            
            SyncGold();
        }

        private void RequestAdd()
        {
            if (AccountManager.I.Gold < requestAddPrice)
                return;
            TileGameManager.I.RequestSystem.RewardLevelAdd();
            AccountManager.I.GoldConsume(requestAddPrice);
            pnlRequestAddComplete.gameObject.SetActive(true);
            
            SyncGold();
        }

        private void RequestComplete()
        {
            pnlRequestReceiveComplete.gameObject.SetActive(true);
            TileGameManager.I.RequestSystem.RequestEnd(out var ret);
            AccountManager.I.AddGold(ret);
            
            SyncGold();
        }


        private void Reroll()
        {
            if (AccountManager.I.Gold < 5)
                return;
            /*foreach (var r in relics)
            {
                if(!r.Purchased)
                    r.Reroll(20, (int)Random.Range(101, (int)RelicName.HorizontalVerticalStabilizer +1), OnClickRelicPack);
            }*/
            
            
            List<int> usedRelic=new List<int>();
            var curRelic = TileGameManager.I.RelicSystem.RelicList;
            if(curRelic != null)
                for (int i = 0; i < curRelic.Count; i++)
                {
                    usedRelic.Add((int)curRelic[i]);
                }
            
            var pack1 = ExcelManager.I.ExecuteGacha(ExcelManager.I.GetShopData(1).gachaId);
            var pack2 = ExcelManager.I.ExecuteGacha(ExcelManager.I.GetShopData(2).gachaId);
            var pack3 = ExcelManager.I.ExecuteGacha(ExcelManager.I.GetShopData(3).gachaId);
            
            //var relic1 = ExcelManager.I.ExecuteGacha(ExcelManager.I.GetShopData(6).gachaId);
            string relic1Key = ExcelManager.I.ExecuteGacha(ExcelManager.I.GetShopData(4).gachaId).rewardValue;
            RelicName relic1Name = relic1Key
                .StringToRelicName();
            while (usedRelic.Contains((int)relic1Name) && usedRelic.Count <9 )
            {
                relic1Key = ExcelManager.I.ExecuteGacha(ExcelManager.I.GetShopData(4).gachaId).rewardValue;
                relic1Name = relic1Key
                    .StringToRelicName();
            }

            bool disableRelic1 = usedRelic.Count >= 9;
            usedRelic.Add((int)relic1Name);
            
            string relic2Key = ExcelManager.I.ExecuteGacha(ExcelManager.I.GetShopData(5).gachaId).rewardValue;
            RelicName relic2Name = relic2Key
                .StringToRelicName();
            while (usedRelic.Contains((int)relic2Name) && usedRelic.Count <9 )
            {
                relic2Key = ExcelManager.I.ExecuteGacha(ExcelManager.I.GetShopData(5).gachaId).rewardValue;
                relic2Name = relic2Key
                    .StringToRelicName();
            }
            bool disableRelic2 = usedRelic.Count >= 9;
            
            /*foreach (var p in packs)
            {
                p.Init(Random.Range(20,31),0, OnClickGachaPack); //todo : 가챠타입, 비용 임시
                p.gameObject.SetActive(true);
            }*/
            
            
            

            if (!disableRelic1&&!relics[0].Purchased)
            {
                //relics[0].gameObject.SetActive(true);
                relics[0].Reroll(ExcelManager.I.GetShopCost(relic1Key), (int)relic1Name, OnClickRelicPack);
            }
            else
            {
                //relics[0].gameObject.SetActive(false);
            }
            
            if (!disableRelic2 && !relics[1].Purchased)
            {
                //relics[1].gameObject.SetActive(true);
                relics[1].Reroll(ExcelManager.I.GetShopCost(relic2Key), (int)relic2Name, OnClickRelicPack);
            }
            else
            {
                //relics[1].gameObject.SetActive(false);
            }


            if (!packs[0].Purchased)
            {
                packs[0].Reroll(ExcelManager.I.GetShopCost(pack1.rewardValue),ExcelManager.I.GetPackData(pack1.rewardValue).gachaKey, OnClickGachaPack);
                packs[0].SetPackResource(ExcelManager.I.GetPackData(pack1.rewardValue).gachaKey-100);
            }
            if (!packs[1].Purchased)
            {
                packs[1].Reroll(ExcelManager.I.GetShopCost(pack2.rewardValue),ExcelManager.I.GetPackData(pack2.rewardValue).gachaKey, OnClickGachaPack);
                packs[1].SetPackResource(ExcelManager.I.GetPackData(pack2.rewardValue).gachaKey-100);
            }
            if (!packs[2].Purchased)
            {
                packs[2].Reroll(ExcelManager.I.GetShopCost(pack3.rewardValue),ExcelManager.I.GetPackData(pack3.rewardValue).gachaKey, OnClickGachaPack);
                packs[2].SetPackResource(ExcelManager.I.GetPackData(pack3.rewardValue).gachaKey-100);
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

            //AccountManager.I.GoldConsume(cost);
            
            UICardPackResultPopup.OpenPopup(gachaType, cost);
            SyncGold();
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

            int cost = 10;
            if (slot == specialCardSlot[0])
                cost = limit1Cost;
            else cost = limit2Cost;
            AccountManager.I.GoldConsume(cost);
            //slot.gameObject.SetActive(false);
            //slot.
            DeckManager.I.TryAddDeck(tool);
            slot.AfterBuy();
            SyncGold();
        }

        protected override void Dispose()
        {
            base.Dispose();
            //SoundManager.I.StopBgm();
        }
    }
}