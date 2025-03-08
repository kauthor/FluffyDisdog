using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Script.FluffyDisdog.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{
    public class UIGameView:UIViewBehaviour
    {
        [SerializeField] private Text txtCurrentTool;
        [SerializeField] private Text txtCurrentScore;
        [SerializeField] private Text txtGoalScore;

        [SerializeField] private Transform cardArea;
        [SerializeField] private CardPart cardPrefab;
        [SerializeField] private int cardSpace = 70;
        public override UIType type => UIType.InGame;

        private List<CardPart> cardPool;

        private List<CardPart> currentCard;

        private int currentSelected = 0;

        [SerializeField]private Transform deckPosition;
        

        public override void Init(UIViewParam param)
        {
            base.Init(param);
            //DeckManager.I.BindHandler(_=> txtCurrentTool.text = _.ToString());
            txtCurrentScore.text = "0";

            var hand = DeckManager.I.Deck.Count;
            var handList = DeckManager.I.Deck;
            
            if (cardPool == null)
                cardPool = new List<CardPart>();
            else cardPool.ForEach(_=>_.gameObject.SetActive(false));

            if (currentCard == null)
                currentCard = new List<CardPart>();
            else currentCard.Clear();

            for (int i = 0; i < hand; i++)
            {
                CardPart current;
                if (cardPool.Count <= i)
                {
                    current = GameObject.Instantiate(cardPrefab, cardArea);
                    cardPool.Add(current);
                }
                else
                {
                    current = cardPool[i];
                }
                
                current.Init(i, handList[i], OnCardClicked);
                current.InitHandler(OnCardHovered, CardSort);
                current.transform.position = //new Vector3(cardSpace * i, 0, 0);
                    deckPosition.transform.position;
                currentCard.Add(current);
            }
            
            CardDraw();

            txtGoalScore.text = TileGameManager.I.LevelData.Goal.ToString();
            TileGameManager.I.SubscribeCurrentScore(RefreshCurrentScore);

            DeckManager.I.BindHandler(DisableCardWhenUsed);
            
            currentSelected = 0;
            currentCard[0].Select(true);
            DeckManager.I.SelectTool(0);
            txtCurrentTool.gameObject.SetActive(true);
            txtCurrentTool.text = DeckManager.I.Deck[0].ToString();
        }

        private async void CardDraw()
        {
            int i = 0;
            CancellationTokenSource token = new CancellationTokenSource();
            foreach (var card in currentCard)
            {
                var target = cardArea.transform.position + new Vector3(cardSpace * i, i==0? 20 : 0, 0);
                card.transform.DOMove(target, 0.5f);
                card.transform.DOScaleX(0, 0.125f)
                    .OnComplete(() =>
                    {
                        card.Flip(true);
                        card.transform.DOScaleX(1, 0.25f);
                    });
                await Task.Delay(83, token.Token);

                
                i++;
            }
        }

        private void OnCardClicked(int id, ToolType type)
        {
            txtCurrentTool.gameObject.SetActive(true);
            txtCurrentTool.text = type.ToString();
            currentSelected = id;
            DeckManager.I.SelectTool(currentSelected);
            for(int i=0; i< currentCard.Count; i ++)
            {
                if(id == i)
                    continue;
                currentCard[i].Select(false);
            }
        }

        private void DisableCardWhenUsed(int id)
        {
            var card = currentCard[id];
            card.gameObject.SetActive(false);
            
            currentSelected = -1;
            txtCurrentTool.gameObject.SetActive(false);
            CardSort(true);
        }

        private void RefreshCurrentScore(int sc)
        {
            txtCurrentScore.text = sc.ToString();
        }

        private void OnCardHovered(int id=0)
        {
            int trId = 0;
            for(int i=0; i< currentCard.Count; i ++)
            {
                if(id == i || !currentCard[i].gameObject.activeSelf)
                    continue;
                currentCard[i].transform.SetSiblingIndex(trId);
                trId++;
            }
            if(currentSelected >=0)
               currentCard[currentSelected].transform.SetAsLastSibling();
            currentCard[id].transform.SetAsLastSibling();
        }

        private void CardSort(bool onCardUsed=false)
        {
            int trId = 0;
            for(int i=0; i< currentCard.Count; i ++)
            {
                if(!currentCard[i].gameObject.activeSelf)
                    continue;
                currentCard[i].transform.SetSiblingIndex(trId);
                
                currentCard[i].transform.localPosition = 
                    new Vector3(cardSpace * trId,currentCard[i].IsSelected? 20 :0, 0);
                trId++;
            }

            if (!onCardUsed)
            {
                if(currentSelected >=0)
                   currentCard[currentSelected].transform.SetAsLastSibling();
            }
        }
    }
}