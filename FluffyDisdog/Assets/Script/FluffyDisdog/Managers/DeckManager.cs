using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluffyDisdog;
using FluffyDisdog.Data.RelicData;
using FluffyDisdog.RelicCommandData;
using FluffyDisdog.UI;
using Sirenix.Utilities;
using UnityEngine;
using Random = System.Random;
using URandom = UnityEngine.Random;

namespace Script.FluffyDisdog.Managers
{
    public class DeckManager:CustomSingleton<DeckManager>
    {
        private ToolType currentType;

        private List<ToolType> deck;
        private bool[] cardUseState;
        public List<ToolType> Deck => deck;

        private List<ToolType> trueDeck;

        [SerializeField] private ToolType[] startDeck = new ToolType[]
        {
            ToolType.Rake, ToolType.Rake, ToolType.Rake, ToolType.Rake, ToolType.Rake, ToolType.Rake, ToolType.Rake,
            ToolType.Rake, ToolType.Shovel, ToolType.Shovel, ToolType.Shovel, ToolType.Shovel, ToolType.Shovel,
            ToolType.Shovel, ToolType.Shovel, ToolType.Shovel
        };

        private Dictionary<ToolType, int> DeckList;

        private event Action<int> onCardUse;

        private int handMax;
        
        
        
        public void Init(int maxHandCard)
        {
            currentType = ToolType.None;
            deck = new List<ToolType>();
            if (trueDeck == null || trueDeck.Count <=0)
            {
                trueDeck = startDeck.ToList();
            }

            if (DeckList == null)
            {
                DeckList = new Dictionary<ToolType, int>();
                trueDeck.ForEach(_ =>
                {
                    if(DeckList.ContainsKey(_))
                    {
                        DeckList[_] = DeckList[_] + 1;
                    }
                    else
                    {
                        DeckList.Add(_,1);
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

        public void TryAddDeck(ToolType tool)
        {
            if (trueDeck != null)
            {
                trueDeck.Add(tool);
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
            deck = new List<ToolType>();

            currentDigged = 0;
            currentSelected = 0;
            var l = Mathf.Min( handMax, trueDeck.Count); 
            //변경필요
            for (int i = 0; i < l; i++)
            {
                deck.Add(trueDeck[i]);
            }

            cardUseState = new bool[l];
            cardUseState.ForEach(_ => _ = false);

        }

        private int currentSelected = 0;
        private int currentDigged = 0;
        public void SelectTool(int id)
        {
            currentSelected = id;
            currentType = deck[id];
            TileGameManager.I.PrepareTool(deck[id]);
        }

        private void OnDigged()
        {
            var param = new ToolConsumeDesire()
            {
                 consumed = true
            };
            PlayerManager.I.TurnEventSystem.FireEvent(TurnEvent.ToolConsumeDesire,param);
            if (param.consumed)
            {
                onCardUse?.Invoke(currentSelected);
                currentDigged++;
                PlayerManager.I.TurnEventSystem.FireEvent(TurnEvent.ToolConsumed, new TurnEventOptionParam());
                cardUseState[currentSelected] = true;
                TileGameManager.I.PrepareTool(ToolType.None);
                currentType = ToolType.None;
            }
            
            if (currentDigged >= deck.Count)
            {
                currentType = ToolType.None;
                var gameEnd = TileGameManager.I.EndStage();
                
                if(!gameEnd)
                    UIStageResultPopup.OpenPopup(false);
                else
                {
                    UIStageRewardPopup.OpenPopup(() =>
                    {
                        TileGameManager.I.GoNextLevel();
                    });
                }
                return;
            }
        }

        public ToolType GetRandomCardFromDeck()
        {
            var rand = new Random();
            return trueDeck[rand.Next() % trueDeck.Count];
        }

        public void RemoveCard(ToolType tool)
        {
            trueDeck.Remove(tool);
            if (DeckList[tool] <= 1)
            {
                DeckList.Remove(tool);
            }
            else
               DeckList[tool] = DeckList[tool] - 1;
            Debug.Log(trueDeck.Count);
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