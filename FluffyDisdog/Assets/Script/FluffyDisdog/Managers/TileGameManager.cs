
using System;
using FluffyDisdog.Manager;
using FluffyDisdog.UI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FluffyDisdog
{
    public enum ToolType
    {
        None=-1,
        Shovel=0,
        Rake=1
    }
    
    public class TileGameManager:CustomSingleton<TileGameManager>
    {
        [SerializeField] private TileSet _tileSet;

        private ToolType currentTool = ToolType.None;

        [Button]
        private void GameStart()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif
            _tileSet.InitGame();
            UIManager.I.ChangeView(UIType.InGame);
        }
        
        public ToolType CurrentTool => currentTool;

        public void BindTileClickedHandler(Action cb)
        {
            _tileSet.BindTileClickedHandler(cb);
        }
        
        //도구의 타입은 나중에 클래스가 될 가능성이 높다.
        public void PrepareTool(ToolType type)
            => currentTool = type;
    }
}

