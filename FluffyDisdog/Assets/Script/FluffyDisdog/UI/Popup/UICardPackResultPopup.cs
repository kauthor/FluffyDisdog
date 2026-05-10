using System;
using System.Collections.Generic;
using Script.FluffyDisdog.Managers;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace FluffyDisdog.UI
{
    public class UICardPackResultPopup:PopupMonoBehavior
    {
        public override PopupType type => PopupType.CardPackResult;


        [SerializeField] private CardPopupParts[] cards;
        [FormerlySerializedAs("btnSkipAndNext")] [SerializeField] private Button btnSkip;
        [SerializeField] private Button btnNext;

        [SerializeField] private Button pnlSkipArea;
        [SerializeField] private Button pnlNextArea;
        //[SerializeField] private Sprite[] skipOrNextSprite;
        [SerializeField] private OutlinedText cardSelectTextSkip;
        [SerializeField] private OutlinedText cardSelectTextNext;
        [SerializeField] private GameObject skipLine;
        [SerializeField] private GameObject nextLine;

        [SerializeField] private Button btnDeck;

        private ToolType[] packResults;

        private int selectLimit = 1;
        
        private List<int> selectedCards = new List<int>();
        
        public static PopupMonoBehavior OpenPopup(int gachaType, int cost)
        {
            var pop = PopupManager.I.GetPopup(PopupType.CardPackResult);
            if (pop is UICardPackResultPopup pack)
            {
                pack.Init(gachaType, cost);
                pack.gameObject.SetActive(true);
                //return pop;
            }

            return pop;
        }

        protected override void Awake()
        {
            base.Awake();
            btnSkip.onClick.RemoveAllListeners();
            btnSkip.onClick.AddListener(() =>
            {
                foreach (var card in selectedCards)
                {
                    DeckManager.I.TryAddDeck(cards[card].ToolType);
                }
                Close();
            });
            btnNext.onClick.RemoveAllListeners();
            btnNext.onClick.AddListener(() =>
            {
                foreach (var card in selectedCards)
                {
                    DeckManager.I.TryAddDeck(cards[card].ToolType);
                }
                Close();
            });
            btnDeck.onClick.RemoveAllListeners();
            btnDeck.onClick.AddListener(() =>
            {
                UIDeckListPopup.OpenPopup();
            });
        }

        private void Init(int gachaType, int cost)
        {
            var result = CutPack(gachaType);
            selectLimit = 1;
            if (result != null && result.Length > 0)
            {
                AccountManager.I.GoldConsume(cost);
                packResults = result;
                for (int i = 0; i < cards.Length; i++)
                {
                    if (result.Length <= i)
                    {
                        cards[i].gameObject.SetActive(false);
                        continue;
                    }
                    cards[i].gameObject.SetActive(true);
                    cards[i].Init(result[i],0);
                    cards[i].BindHandler(OnCardClicked);
                    cards[i].InitAsSelectable(()=>
                        selectedCards.Count < selectLimit);
                }
            }
            cardSelectTextSkip.SetText($"(0/{selectLimit})");
            cardSelectTextNext.SetText($"(0/{selectLimit})");
        }

        private ToolType[] CutPack(int gachaType)
        {
            //todo:임시
            ToolType[] ret = new ToolType[3];
            for (int i = 0; i < 3; i++)
            {
                var gacha= ExcelManager.I.ExecuteGacha(gachaType).rewardValue;
                ret[i] = (ToolType)System.Enum.Parse(typeof(ToolType), gacha);
            }
            

            return ret;
        }

        private void OnCardClicked(ToolType t, CardPopupParts c)
        {
            var n = Array.IndexOf(cards, c);
            if (c.Selected)
            {
                if(selectedCards.Count < selectLimit)
                   selectedCards.Add(n);
            }
            else
            {
                selectedCards.Remove(n);
            }
            
            cardSelectTextSkip.SetText($"({selectedCards.Count}/{selectLimit})");
            cardSelectTextNext.SetText($"({selectedCards.Count}/{selectLimit})");
            bool activeSkip = selectedCards.Count <= 0;
            pnlSkipArea.gameObject.SetActive(activeSkip);
            pnlNextArea.gameObject.SetActive(!activeSkip);
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