using System;
using System.Collections.Generic;
using Script.FluffyDisdog.Managers;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace FluffyDisdog.UI
{
    public class UICardPackResultPopup:PopupMonoBehavior
    {
        public override PopupType type => PopupType.CardPackResult;


        [SerializeField] private CardPopupParts[] cards;
        [SerializeField] private Button btnSkipAndNext;
        [SerializeField] private Sprite[] skipOrNextSprite;
        [SerializeField] private OutlinedText cardSelectText;

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
            btnSkipAndNext.onClick.RemoveAllListeners();
            btnSkipAndNext.onClick.AddListener(() =>
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
            cardSelectText.SetText($"(0/{selectLimit})");
        }

        private ToolType[] CutPack(int gachaType)
        {
            //todo:임시
            var ret = new ToolType[3];
            for (int i = 0; i < 3; i++)
            {
                var num = Random.Range(0, 10);
                ret[i] = (ToolType)num;
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
            
            cardSelectText.SetText($"({selectedCards.Count}/{selectLimit})");
            btnSkipAndNext.image.sprite = skipOrNextSprite[selectedCards.Count<=0?0:1];
        }
        
    }
}