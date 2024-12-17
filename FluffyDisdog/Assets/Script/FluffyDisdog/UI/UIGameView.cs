using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FluffyDisdog.UI
{
    public class UIGameView:UIViewBehaviour
    {
        [SerializeField] private Text txtCurrentTool;
        public override UIType type => UIType.InGame;

        private ToolType currentType;

        private Stack<ToolType> deck;
        public override void Init(UIViewParam param)
        {
            base.Init(param);
            currentType = ToolType.None;
            deck = new Stack<ToolType>();

            //일단은 타일을 클릭하면 드로우 하게 하자
            TileGameManager.I.BindTileClickedHandler(DrawDeck);
            
            SetDeck();
            DrawDeck();
        }

        private void SetDeck()
        {
            deck = new Stack<ToolType>();

            var deckLength = 30; //변경필요
            for (int i = 0; i < deckLength; i++)
            {
                deck.Push((ToolType)Random.Range(0,1));
            }
        }

        private void DrawDeck()
        {
            if(deck==null || deck.Count<=0)
                SetDeck(); //우선 무한리필한다
            currentType = deck.Pop();
            txtCurrentTool.text = currentType.ToString();
            if(TileGameManager.ExistInstance())
                TileGameManager.I.PrepareTool(currentType);
        }
    }
}