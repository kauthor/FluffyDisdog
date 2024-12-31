using System;
using System.Collections;
using System.Collections.Generic;
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

        public override UIType type => UIType.InGame;

        private List<CardPart> cardPool;

        private List<CardPart> currentCard;

        private int currentSelected = 0;
        

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
                currentCard.Add(current);
            }
            
            txtGoalScore.text = TileGameManager.I.LevelData.Goal.ToString();
            TileGameManager.I.SubscribeCurrentScore(RefreshCurrentScore);

            DeckManager.I.BindHandler(DisableCardWhenUsed);
            
            currentSelected = 0;
            currentCard[0].Select(true);
            DeckManager.I.SelectTool(0);
            txtCurrentTool.gameObject.SetActive(true);
            txtCurrentTool.text = DeckManager.I.Deck[0].ToString();
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
            
            currentSelected = 0;
            txtCurrentTool.gameObject.SetActive(false);
        }

        private void RefreshCurrentScore(int sc)
        {
            txtCurrentScore.text = sc.ToString();
        }
    }
}