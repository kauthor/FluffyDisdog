using System;
using System.Collections.Generic;
using System.Linq;
using Script.FluffyDisdog.Managers;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace FluffyDisdog.UI
{
    public class UIStageRewardPopup : PopupMonoBehavior
    {
        public override PopupType type => PopupType.Reward;

        [SerializeField] private CardPopupParts[] cards;
        [SerializeField] private Button btnSelectRemoveFromDeck;

        [SerializeField] private Button btnReroll;
        [FormerlySerializedAs("btnSkipAndNext")] [SerializeField] private Button btnSkip;
        [SerializeField] private Button btnNext;

        [SerializeField] private Button pnlSkipArea;
        [SerializeField] private Button pnlNextArea;
        //[SerializeField] private Sprite[] skipOrNextSprite;
        [SerializeField] private OutlinedText cardSelectTextSkip;
        [SerializeField] private OutlinedText cardSelectTextNext;
        [SerializeField] private GameObject skipLine;
        [SerializeField] private GameObject nextLine;

        [SerializeField] private OutlinedText rerollCountText;
        //[SerializeField] private OutlinedText cardSelectText;
        
        [SerializeField] private CanvasGroup canvasGroup;

        private Action OnClosedCb;

        private List<ToolType> currentSelected;

        private int cardLimit;

        private int rerollLimit = 2;
        private int rerollCount = 0;
        

        public static void OpenPopup(Action onClosed, int cardLimit=1)
        {
            var pop = PopupManager.I.GetPopup(PopupType.Reward);
            if (pop is UIStageRewardPopup re)
            {
                re.gameObject.SetActive(true);
                re.Init(onClosed, cardLimit);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            btnReroll.onClick.RemoveAllListeners();
            btnReroll.onClick.AddListener(() =>
            {
                if (rerollCount >= rerollLimit)
                    return;
                rerollCount++;
                currentSelected.Clear();
                rerollCountText.SetText($"({rerollLimit-rerollCount}/{rerollLimit})");
                cardSelectTextNext.SetText($"(0/{cardLimit})");
                cardSelectTextSkip.SetText($"(0/{cardLimit})");
                List<ToolType> appeared = new List<ToolType>();
                
                for (int i = 0; i < cards.Length ; i++)
                {
                    var newTool = (ToolType)(SeedManager.I.GetStoreSeed() % 7);
                    while (appeared.Contains(newTool))
                    {
                        newTool = (ToolType)(SeedManager.I.GetStoreSeed() % 7);
                    }
                    appeared.Add(newTool);
                    cards[i].Init(newTool, 0);
                    cards[i].BindHandler((a, b) =>
                    {
                        OnCardClicked(a,b);
                    });
                    cards[i].InitAsSelectable(()=> currentSelected.Count < cardLimit);
                }
                pnlSkipArea.gameObject.SetActive(true);
                pnlNextArea.gameObject.SetActive(false);
                //cards[^1].SetCardData(DeckManager.I.GetRandomCardFromDeck());
            });
            
            btnSelectRemoveFromDeck.onClick.RemoveAllListeners();
            btnSelectRemoveFromDeck.onClick.AddListener(() =>
            {
                canvasGroup.alpha = 0;
                UIDeckListPopup.OpenPopup(false, ()=>
                {
                    canvasGroup.alpha = 1;
                });
            });
            
            btnNext.onClick.RemoveAllListeners();
            btnNext.onClick.AddListener(() =>
            {
                foreach (var card in currentSelected)
                {
                    DeckManager.I.TryAddDeck(card);
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
                Close();
            });
        }

        private void Init(Action onclosed, int cardLimit=1)
        {
            List<ToolType> appeared = new List<ToolType>();
            for (int i = 0; i < cards.Length ; i++)
            {
                var newTool = (ToolType)(SeedManager.I.GetStoreSeed() % 7);
                while (appeared.Contains(newTool))
                {
                    newTool = (ToolType)(SeedManager.I.GetStoreSeed() % 7);
                }
                appeared.Add(newTool);
                cards[i].Init( newTool, 0);
                cards[i].BindHandler((a, b) =>
                {
                    OnCardClicked(a,b);
                });
                cards[i].InitAsSelectable(()=> currentSelected.Count < cardLimit);
            }
            //cards[^1].InitAsReroll(OnRerollCardClicked, DeckManager.I.GetRandomCardFromDeck());
            OnClosedCb = onclosed;
            this.cardLimit = cardLimit;
            currentSelected = new List<ToolType>();
            cardSelectTextNext.SetText($"(0/{cardLimit})");
            cardSelectTextSkip.SetText($"(0/{cardLimit})");
            rerollCountText.SetText($"({rerollLimit}/{rerollLimit})");
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
                if (currentSelected.Count < cardLimit)
                    currentSelected.Add(tool);
            }
            else
            {
                currentSelected.Remove(tool);
            }
            
            cardSelectTextNext.SetText($"({currentSelected.Count}/{cardLimit})");
            cardSelectTextSkip.SetText($"({currentSelected.Count}/{cardLimit})");
            bool skip = currentSelected.Count <= 0;
            pnlSkipArea.gameObject.SetActive(skip);
            pnlNextArea.gameObject.SetActive(!skip);
        }

        private void OnRerollCardClicked(ToolType tool)
        {
            DeckManager.I.RemoveCard(tool);
            Close();
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