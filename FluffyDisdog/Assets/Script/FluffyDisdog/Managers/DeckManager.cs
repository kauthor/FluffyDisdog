using System;
using System.Collections;
using System.Collections.Generic;
using FluffyDisdog;
using FluffyDisdog.UI;
using Sirenix.Utilities;
using Random = UnityEngine.Random;

namespace Script.FluffyDisdog.Managers
{
    public class DeckManager:CustomSingleton<DeckManager>
    {
        private ToolType currentType;

        private List<ToolType> deck;
        private bool[] cardUseState;
        public List<ToolType> Deck => deck;

        private event Action<int> onCardUse;

        private int handMax;
        
        
        
        public void Init(int maxHandCard)
        {
            currentType = ToolType.None;
            deck = new List<ToolType>();

            //일단은 타일을 클릭하면 드로우 하게 하자
            TileGameManager.I.BindTileClickedHandler(OnDigged);
            onCardUse = null;
            handMax = maxHandCard;
            SetHand();
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
            var l = handMax; 
            //변경필요
            for (int i = 0; i < l; i++)
            {
                deck.Add((ToolType)Random.Range(0,2));
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
            onCardUse?.Invoke(currentSelected);
            currentDigged++;
            cardUseState[currentSelected] = true;
            TileGameManager.I.PrepareTool(ToolType.None);
            currentType = ToolType.None;
            if (currentDigged >= deck.Count)
            {
                currentType = ToolType.None;
                var gameEnd = TileGameManager.I.EndStage();
                
                UIStageResultPopup.OpenPopup(gameEnd);
                
                return;
            }
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
    }
}