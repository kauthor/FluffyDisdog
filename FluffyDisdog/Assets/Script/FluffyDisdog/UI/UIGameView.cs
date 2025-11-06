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
        
        [SerializeField] private BinaryGoldPrefab[] goldText;
        public override UIType type => UIType.InGame;

        private List<CardPart> cardPool;

        private List<CardPart> currentCard;

        private int currentSelected = 0;

        [SerializeField]private Transform deckPosition;
        [SerializeField] private Button btnDeckList;

        private Queue<UIRelicInfoPart> relicPool;
        private Queue<UIRelicInfoPart> currentRelic;
        
        [SerializeField] private UIRelicInfoPart relicPrefab;
        [SerializeField] private Transform relicParent;

        [SerializeField] private Button btnOption;
        
        private void Awake()
        {
            
            
            relicPool = new Queue<UIRelicInfoPart>();
            currentRelic = new Queue<UIRelicInfoPart>();
        }

        private void Start()
        {
            btnDeckList.onClick.RemoveAllListeners();
            btnDeckList.onClick.AddListener(UIDeckListPopup.OpenPopup);
            btnOption.onClick.RemoveAllListeners();
            btnOption.onClick.AddListener(UIOptionPopup.OpenPopup);
        }

        private void SyncGold()
        {
            int gold = AccountManager.I.Gold;

            for (int i = 0; i < goldText.Length; i++)
            {
                goldText[i].SetBinary(gold%10);
                gold = gold/10;
            }
        }

        public override void Init(UIViewParam param)
        {
            base.Init(param);
            //DeckManager.I.BindHandler(_=> txtCurrentTool.text = _.ToString());
            txtCurrentScore.text = "0";

            var hand = DeckManager.I.Hand.Count;
            var handList = DeckManager.I.Hand;
            
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
                
                current.Init(i, handList[i].ToolType, OnCardClicked, OnCardClickCancel);
                current.InitHandler(OnCardHovered, CardSort);
                current.transform.position = //new Vector3(cardSpace * i, 0, 0);
                    deckPosition.transform.position;
                currentCard.Add(current);
                current.gameObject.SetActive(false);
            }
            
            
            
            CardDraw();

            txtGoalScore.text = TileGameManager.I.LevelData.Goal.ToString();
            TileGameManager.I.SubscribeCurrentScore(RefreshCurrentScore);

            DeckManager.I.BindHandler(DisableCardWhenUsed);

            var relics = TileGameManager.I.RelicSystem.currentRelicDatas;

            if(relics != null && relics.Length > 0)
                foreach (var relic in relics)
                {
                    if (relicPool.Count > 0)
                    {
                        var current = relicPool.Dequeue();
                        current.gameObject.SetActive(true);
                        current.InitData(relic.relicName);
                        currentRelic.Enqueue(current);
                    }
                    else
                    {
                        var newRelic = GameObject.Instantiate(relicPrefab, relicParent);
                        newRelic.InitData(relic.relicName);
                        currentRelic.Enqueue(newRelic);
                    }
                }
            
            SyncGold();
        }

        private async void CardDraw()
        {
            int i = 0;
            CancellationTokenSource token = new CancellationTokenSource();
            foreach (var card in currentCard)
            {
                card.transform.position = //new Vector3(cardSpace * i, 0, 0);
                    deckPosition.transform.position;
                card.gameObject.SetActive(true);
                var target = cardArea.transform.position + new Vector3(cardSpace * (i+1), -250, 0);
                //card.transform.SetAsLastSibling();
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
            currentSelected = -1;
            //currentCard[0].Select(true);
            //DeckManager.I.SelectTool(0);
            //txtCurrentTool.gameObject.SetActive(true);
            //txtCurrentTool.text = DeckManager.I.Hand[0].ToolType.ToString();
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

        private void OnCardClickCancel()
        {
            txtCurrentTool.gameObject.SetActive(false);
            currentSelected = -1;
            TileGameManager.I.PrepareTool(ToolType.None,-1);
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
                //currentCard[i].transform.SetSiblingIndex(trId);
                trId++;
            }
            //if(currentSelected >=0)
            //   currentCard[currentSelected].transform.SetAsLastSibling();
            //currentCard[id].transform.SetAsLastSibling();
        }

        private void CardSort(bool onCardUsed=false)
        {
            int trId = 0;
            for(int i=0; i< currentCard.Count; i ++)
            {
                if(!currentCard[i].gameObject.activeSelf)
                    continue;
                //currentCard[i].transform.SetSiblingIndex(trId);
                
                currentCard[i].transform.localPosition = 
                    new Vector3(cardSpace * (trId+1),currentCard[i].IsSelected? -140 :-250, 0);
                trId++;
            }

            if (!onCardUsed)
            {
                //if(currentSelected >=0)
                //   currentCard[currentSelected].transform.SetAsLastSibling();
            }
        }

        protected override void Dispose()
        {
            base.Dispose();
            foreach (var re in currentRelic)
            {
                re.ReturnToPool();
                relicPool.Enqueue(re);
            }
        }
    }
}