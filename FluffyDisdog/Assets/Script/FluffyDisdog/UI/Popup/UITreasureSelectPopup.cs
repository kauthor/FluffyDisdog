using System;
using System.Collections.Generic;
using FluffyDisdog.Data.RelicData;
using Script.FluffyDisdog.Managers;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{
    public class UITreasureSelectPopup:PopupMonoBehavior
    {
        public override PopupType type => PopupType.TreasureSelect;
        
        [SerializeField] private CardPopupParts[] cards;
        [SerializeField] private GoldSlotTreasure gold;
        [SerializeField] private RelicSlotTreasure relic;
        
        [FormerlySerializedAs("btnSkipAndNext")] [SerializeField] private Button btnSkip;
        [SerializeField] private Button btnNext;

        [SerializeField] private Button pnlSkipArea;
        [SerializeField] private Button pnlNextArea;
        [SerializeField] private OutlinedText cardSelectTextSkip;
        [SerializeField] private OutlinedText cardSelectTextNext;
        [SerializeField] private GameObject skipLine;
        [SerializeField] private GameObject nextLine;
        
        [SerializeField] private CanvasGroup canvasGroup;

        private Action OnClosedCb;

        private List<ToolType> currentSelected;
        private List<int> currentGoldSelected;
        private List<RelicName> currentRelicSelected;

        private int selectedAmount=0;

        private int cardLimit;

        private int rerollLimit = 2;
        private int rerollCount = 0;
        

        public static void OpenPopup(Action onClosed, int treasureDegree=1)
        {
            var pop = PopupManager.I.GetPopup(PopupType.TreasureSelect);
            if (pop is UITreasureSelectPopup re)
            {
                re.gameObject.SetActive(true);
                re.Init(onClosed, treasureDegree);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            
            btnNext.onClick.RemoveAllListeners();
            btnNext.onClick.AddListener(() =>
            {
                
                Close();
            });
            btnSkip.onClick.RemoveAllListeners();
            btnSkip.onClick.AddListener(() =>
            {
                
                Close();
            });
        }

        private void Init(Action onclosed, int treasureDegree=1)
        {
            //List<ToolType> appeared = new List<ToolType>();
            var mapData = ExcelManager.I.GetMapData(1);
            
            
            ToolType[] rewards = new ToolType[5];
            for (int i = 0; i < rewards.Length; i++)
            {
                //todo : 아무래도 별도의 테이블이 있겠다만... 일단은 기존 스테이지 보상 풀을 쓰자.
                rewards[i] = (ToolType)System.Enum.Parse(typeof(ToolType), (ExcelManager.I.ExecuteGacha(mapData.stageCardRewardGachaId).rewardValue));
            }
            for (int i = 0; i < cards.Length ; i++)
            {
                
                if (i >= rewards.Length)
                {
                    cards[i].gameObject.SetActive(false);
                    continue;
                }
                    
                cards[i].gameObject.SetActive(true);
                cards[i].Init(rewards[i],0);
                cards[i].BindHandler((a, b) =>
                {
                    OnCardClicked(a,b);
                });
                cards[i].InitAsSelectable(()=>
                {
                    return currentSelected !=null && selectedAmount < cardLimit;
                });
            }
            
            gold.Init(20, OnGoldClicked);
            gold.InitAsSelectable(() =>
            {
                return currentGoldSelected != null && selectedAmount < cardLimit;
            });
            
            relic.Init(RelicName.AncientCompass, OnRelicClicked);
            relic.InitAsSelectable(() =>
            {
                return currentRelicSelected != null && selectedAmount < cardLimit;
            });
            
            btnNext.onClick.RemoveAllListeners();
            btnNext.onClick.AddListener(() =>
            {
                foreach (var card in currentSelected)
                {
                    DeckManager.I.TryAddDeck(card);
                }

                foreach (var g in currentGoldSelected)
                {
                    AccountManager.I.AddGold(g);
                }

                foreach (var r in currentRelicSelected)
                {
                    TileGameManager.I.RelicSystem.GainRelicWhileGame(r);
                }
                Close();
            });
            btnSkip.onClick.RemoveAllListeners();
            btnSkip.onClick.AddListener(() =>
            {
                foreach (var card in currentSelected)
                {
                    DeckManager.I.TryAddDeck(card);
                }
                foreach (var g in currentGoldSelected)
                {
                    AccountManager.I.AddGold(g);
                }

                foreach (var r in currentRelicSelected)
                {
                    TileGameManager.I.RelicSystem.GainRelicWhileGame(r);
                }
                Close();
            });
            
            //cards[^1].InitAsReroll(OnRerollCardClicked, DeckManager.I.GetRandomCardFromDeck());
            OnClosedCb = onclosed;
            this.cardLimit = 2; //임시
            
            currentSelected = new List<ToolType>();
            currentGoldSelected = new List<int>();
            currentRelicSelected = new List<RelicName>();
            
            cardSelectTextNext.SetText($"(0/{cardLimit})");
            cardSelectTextSkip.SetText($"(0/{cardLimit})");
        }

        protected override void OnCloseClick()
        {
            base.OnCloseClick();
            OnClosedCb?.Invoke();
        }

        private void OnCardClicked(ToolType tool, CardPopupParts card)
        {
            //DeckManager.I.TryAddDeck(tool);
            //Close();
            if(card.Selected)
            {
                if (selectedAmount < cardLimit)
                {
                    currentSelected.Add(tool);
                    selectedAmount++;
                }
            }
            else
            {
                currentSelected.Remove(tool);
                selectedAmount--;
            }
            
            cardSelectTextNext.SetText($"({selectedAmount}/{cardLimit})");
            cardSelectTextSkip.SetText($"({selectedAmount}/{cardLimit})");
            bool skip = currentSelected.Count <= 0;
            pnlSkipArea.gameObject.SetActive(skip);
            pnlNextArea.gameObject.SetActive(!skip);
        }

        private void OnGoldClicked(int amount, GoldSlotTreasure card)
        {
            if(card.Selected)
            {
                if (selectedAmount < cardLimit)
                {
                    selectedAmount++;
                    currentGoldSelected.Add(amount);
                }
            }
            else
            {
                currentGoldSelected.Remove(amount);
                selectedAmount--;
            }
            
            cardSelectTextNext.SetText($"({selectedAmount}/{cardLimit})");
            cardSelectTextSkip.SetText($"({selectedAmount}/{cardLimit})");
            bool skip = selectedAmount <= 0;
            pnlSkipArea.gameObject.SetActive(skip);
            pnlNextArea.gameObject.SetActive(!skip);
        }

        private void OnRelicClicked(RelicName rel, RelicSlotTreasure card)
        {
            if(card.Selected)
            {
                if (selectedAmount < cardLimit)
                {
                    selectedAmount++;
                    currentRelicSelected.Add(rel);
                }
            }
            else
            {
                currentRelicSelected.Remove(rel);
                selectedAmount--;
            }
            
            cardSelectTextNext.SetText($"({selectedAmount}/{cardLimit})");
            cardSelectTextSkip.SetText($"({selectedAmount}/{cardLimit})");
            bool skip = selectedAmount <= 0;
            pnlSkipArea.gameObject.SetActive(skip);
            pnlNextArea.gameObject.SetActive(!skip);
        }
        
        public void MouseHoverSkip(bool hover)
        {
            skipLine.SetActive(hover);
        }
        public void MouseHoverNext(bool hover)
        {
            nextLine.SetActive(hover);
        }
    }
}