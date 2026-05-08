using System;
using System.Collections.Generic;
using System.Linq;
using Script.FluffyDisdog.Managers;
using Sirenix.Utilities;
using UnityEngine;
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
        [SerializeField] private Button btnSkipAndNext;
        [SerializeField] private Sprite[] skipOrNextSprite;

        [SerializeField] private OutlinedText rerollCountText;
        [SerializeField] private OutlinedText cardSelectText;

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
                rerollCountText.SetText($"({rerollLimit-rerollCount}/{rerollLimit})");
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
                //cards[^1].SetCardData(DeckManager.I.GetRandomCardFromDeck());
            });
            
            btnSelectRemoveFromDeck.onClick.RemoveAllListeners();
            btnSelectRemoveFromDeck.onClick.AddListener(() =>
            {
                UIDeckSelectPopup.OpenPopup(_ =>
                {
                    DeckManager.I.RemoveCard(_);
                    Close();
                });
            });
            
            btnSkipAndNext.onClick.RemoveAllListeners();
            btnSkipAndNext.onClick.AddListener(() =>
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
            cardSelectText.SetText($"(0/{cardLimit})");
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
            
            cardSelectText.SetText($"({currentSelected.Count}/{cardLimit})");
            btnSkipAndNext.image.sprite = skipOrNextSprite[currentSelected.Count<=0?0:1];
        }

        private void OnRerollCardClicked(ToolType tool)
        {
            DeckManager.I.RemoveCard(tool);
            Close();
        }
    }
}