using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluffyDisdog;
using FluffyDisdog.CardOptionExecuter;
using FluffyDisdog.Data;
using FluffyDisdog.Data.RelicData;
using FluffyDisdog.Manager;
using FluffyDisdog.RelicCommandData;
using FluffyDisdog.UI;
using Sirenix.Utilities;
using UnityEngine;
using Random = System.Random;
using URandom = UnityEngine.Random;

namespace Script.FluffyDisdog.Managers
{
    public class CardInGame
    {
        private ToolType _toolType;
        public ToolType ToolType => _toolType;

        private int cardUsedCount = 0;
        public int CardUsedCount => cardUsedCount;
        
        private CardOptionExecuter executer;
        public CardOptionExecuter Executor => executer;
        private int deckId;
        public int DeckId => deckId;

        private ToolCardOpData rawOpData;
        private ToolExcelData excelData;

        public CardInGame(ToolType toolType, int deckId)
        {
            _toolType = toolType;
            var cardData = ExcelManager.I.GetToolCardOpData(this._toolType);
            excelData = ExcelManager.I.GetToolExcelData(this._toolType);
            if (cardData != null)
            {
                rawOpData = cardData;
                executer = CardOptionExecuter.MakeCardAddOptionExecuter(cardData);
            }
            //executer.InitCommandData();
            this.deckId = deckId;
        }

        public void OnCardUsed()
        {
            //executer?.ExecuteCommand();
            
            cardUsedCount++;
            if (excelData != null)
            {
                var tag = excelData.ToolTag;
                if (((int)tag & 128) != 0)
                {
                    DeckManager.I.RemoveCard(this);
                }
                else if (((int)tag & 32) != 0)
                {
                    var seed = SeedManager.I.GetMinor();
                    var rand = seed % 10000;
                    if (rand  <  Mathf.Min(cardUsedCount,14)*500)
                    {
                        DeckManager.I.RemoveCard(this);
                    }
                }
            }
            //todo : 여기서 카드 사용 판정내자
        }
    }
    public class DeckManager:CustomSingleton<DeckManager>
    {
        private ToolType currentType;

        private List<CardInGame> hand;
        private bool[] cardUseState;
        public List<CardInGame> Hand => hand;

        private List<CardInGame> trueDeck;

        [SerializeField] private ToolType[] startDeck = new ToolType[]
        {
            (ToolType)1,(ToolType)1,(ToolType)1,(ToolType)1,(ToolType)1,(ToolType)1,(ToolType)1,(ToolType)1,
            (ToolType)2,(ToolType)2,(ToolType)2,(ToolType)2,(ToolType)2,(ToolType)2,(ToolType)2,(ToolType)2,
        };

        private Dictionary<ToolType, int> DeckList;

        private event Action<int> onCardUse;

        private int handMax;

        private int usedId = 0;

        private CardInGame currentCard;
        public CardInGame CurrentCard => currentCard; 
        
        public void Init(int maxHandCard)
        {
            currentType = ToolType.None;
            hand = new List<CardInGame>();
            usedId = 0;
            if (trueDeck == null || trueDeck.Count <=0)
            {
                trueDeck = new List<CardInGame>();
                foreach (var item in startDeck)
                {
                    trueDeck.Add(new CardInGame(item,usedId++));
                }
                
            }

            if (DeckList == null)
            {
                DeckList = new Dictionary<ToolType, int>();
                trueDeck.ForEach(_ =>
                {
                    if(DeckList.ContainsKey(_.ToolType))
                    {
                        DeckList[_.ToolType] = DeckList[_.ToolType] + 1;
                    }
                    else
                    {
                        DeckList.Add(_.ToolType,1);
                    }
                });
            }

            var rand = new Random();
            trueDeck = trueDeck.OrderBy(_ => rand.Next()).ToList();

            //일단은 타일을 클릭하면 드로우 하게 하자
            TileGameManager.I.BindTileClickedHandler(OnDigged);
            onCardUse = null;
            handMax = maxHandCard + (PlayerManager.I.TurnEventSystem.HasRelicCommand(RelicName.ExpandedBackpack) ? 1:0);
            SetHand();
        }

        public void PreEffect(CardExecuteParam param)
        {
            if(currentCard!=null)
                currentCard.Executor.PreEffect(param);
        }

        public void OnEffect(CardExecuteParam param)
        {
            if(currentCard!=null)
                currentCard.Executor.ExecuteTileEffect(param);
        }
        
        public void PostEffect(CardExecuteParam param)
        {
            if(currentCard!=null)
                currentCard.Executor.PostEffect(param);
        }

        public void TryAddDeck(ToolType tool)
        {
            if (trueDeck != null)
            {
                var newCard = new CardInGame(tool, usedId++);
                trueDeck.Add(newCard);
                if (DeckList.ContainsKey(tool))
                {
                    DeckList[tool] = DeckList[tool] + 1;
                }
                else
                {
                    DeckList.Add(tool,1);
                }
            }
        }

        //public void StartGameAfterInit() => Draw();

        public void BindHandler(Action<int> cb)
        {
            onCardUse -= cb;
            onCardUse += cb;
        }
        
        private void SetHand()
        {
            hand = new List<CardInGame>();

            currentDigged = 0;
            currentSelected = 0;
            var l = Mathf.Min( handMax, trueDeck.Count); 
            //변경필요
            for (int i = 0; i < l; i++)
            {
                PlayerManager.I.TurnEventSystem.FireEvent(TurnEvent.Draw, new DrawParam()
                {
                    toolType = trueDeck[i].ToolType
                });
                hand.Add(trueDeck[i]);
            }

            cardUseState = new bool[l];
            cardUseState.ForEach(_ => _ = false);

        }

        private int currentSelected = 0;
        private int currentDigged = 0;
        public void SelectTool(int id)
        {
            currentSelected = id;
            currentType = hand[id].ToolType;
            currentCard = hand[id];
            TileGameManager.I.PrepareTool(hand[id].ToolType, hand[id].DeckId);
        }

        private void OnDigged()
        {
            var param = new ToolConsumeDesire()
            {
                 consumed = true
            };
            //currentCard.
            PlayerManager.I.TurnEventSystem.FireEvent(TurnEvent.ToolConsumeDesire,param);
            if (param.consumed)
            {
                onCardUse?.Invoke(currentSelected);
                currentDigged++;
                PlayerManager.I.TurnEventSystem.FireEvent(TurnEvent.ToolConsumed, new TurnEventOptionParam());
                cardUseState[currentSelected] = true;
                TileGameManager.I.PrepareTool(ToolType.None,-1);
                currentType = ToolType.None;
            }
            
            if (currentDigged >= hand.Count)
            {
                currentType = ToolType.None;
                var gameEnd = TileGameManager.I.EndStage();
                
                if(!gameEnd)
                    UIStageResultPopup.OpenPopup(false);
                else
                {
                    UIStageRewardPopup.OpenPopup(() =>
                    {
                        //TileGameManager.I.GoNextLevel();
                        UIManager.I.ChangeView(UIType.Store);
                    });
                }
                return;
            }
        }

        /*public ToolType GetRandomCardFromDeck()
        {
            var rand = new Random();
            return trueDeck[rand.Next() % trueDeck.Count];
        }*/

        public void RemoveCard(ToolType tool)
        {
            var target = trueDeck.First(_=>_.ToolType == tool);
            if(target != null)
               trueDeck.Remove(target);
            if (DeckList[tool] <= 1)
            {
                DeckList.Remove(tool);
            }
            else
               DeckList[tool] = DeckList[tool] - 1;
            Debug.Log(trueDeck.Count);
        }

        public void RemoveCard(CardInGame card)
        {
            if(trueDeck.Contains(card))
                trueDeck.Remove(card);
        }

        /*private void Draw()
        {
            if (deck == null || deck.Count <= 0)
            {
                currentType = ToolType.None;
                var gameEnd = TileGameManager.I.EndStage();
                
                UIStageResultPopup.OpenPopup(gameEnd);
                
                return;
            }
            
            currentType = deck.Pop();
            onCardDraw?.Invoke(currentType);
            if(TileGameManager.ExistInstance())
                TileGameManager.I.PrepareTool(currentType);
        }*/

        public Dictionary<ToolType, int> GetDeckList() => DeckList;
    }
}