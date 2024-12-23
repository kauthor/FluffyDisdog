using System;
using System.Collections.Generic;
using FluffyDisdog;
using FluffyDisdog.UI;
using Random = UnityEngine.Random;

namespace Script.FluffyDisdog.Managers
{
    public class DeckManager:CustomSingleton<DeckManager>
    {
        private ToolType currentType;

        private Stack<ToolType> deck;

        private event Action<ToolType> onCardDraw;

        private int handMax;
        
        public void Init(int maxHandCard)
        {
            currentType = ToolType.None;
            deck = new Stack<ToolType>();

            //일단은 타일을 클릭하면 드로우 하게 하자
            TileGameManager.I.BindTileClickedHandler(Draw);
            onCardDraw = null;
            handMax = maxHandCard;
            SetHand();
        }

        public void StartGameAfterInit() => Draw();

        public void BindHandler(Action<ToolType> cb)
        {
            onCardDraw -= cb;
            onCardDraw += cb;
        }
        
        private void SetHand()
        {
            deck = new Stack<ToolType>();

            var l = handMax; 
            //변경필요
            for (int i = 0; i < l; i++)
            {
                deck.Push((ToolType)Random.Range(0,2));
            }
            
        }

        private void Draw()
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
        }
    }
}