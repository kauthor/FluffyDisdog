
using System;
using FluffyDisdog.Manager;
using FluffyDisdog.UI;
using Script.FluffyDisdog.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FluffyDisdog
{
    public enum ToolType
    {
        None=-1,
        Shovel=0,
        Rake=1,
        NewTool1=2,
        NewTool2=3,
        NewTool3=4,
        NewTool4=5,
        NewTool5=6,
        NewTool6=7,
        NewTool7=8,
        NewTool8=9,
        NewTool9=10,
        NewTool10=11,
        NewTool11=12,
        NewTool12=13,
        NewTool13=14,
        NewTool14=15,
        NewTool15=16,
        NewTool16=17,
        NewTool17=18,
        NewTool18=19,
        NewTool19=20,
        NewTool20=21,
        NewTool21=22,
        NewTool22=23,
        NewTool23=24,
        NewTool24=25,
        NewTool25=26,
        NewTool26=27,
        NewTool27=28,
        NewTool28=29,
        NewTool29=30,
        NewTool30=31,
        NewTool31=32,
        NewTool32=33,
        NewTool33=34,
        NewTool34=35,
        NewTool35=36,
        NewTool36=37,
        NewTool37=38,
        NewTool38=39,
        NewTool39=40,
        NewTool40=41,
        NewTool41=42,
        NewTool42=43,
        NewTool43=44,
        NewTool44=45,
        NewTool45=46,
        NewTool46=47,
        NewTool47=48,
        NewTool48=49,
        NewTool49=50,
        MAX=51
    }

    public class LevelData
    {
        private int goal;
        public int Goal => goal;
        private int maxHandCard;
        public int MaxHandCard => maxHandCard;

        public LevelData(int g, int m)
        {
            goal = g;
            maxHandCard = m;
        }
    }

    public class TileGameManager:CustomSingleton<TileGameManager>
    {
        [SerializeField] private TileSet _tileSet;

        public TileSet TileSet => _tileSet;

        private ToolType currentTool = ToolType.None;

        private LevelData _levelData;
        public LevelData LevelData => _levelData;
        private IntReactiveFluffyProperty currentScore;
        private bool isGameRunning=false;

        public bool IsGameRunning => isGameRunning;

        public int currentLevel = 1;

        [Button]
        private async void GameStart(int level=1)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif
            var load = UILoadingPopup.NormalLoadStart();
            currentLevel = level;
            
            //임의로 최대레벨 설정
            if (currentLevel > 2)
                currentLevel = 1;
            await _tileSet.InitGame(currentLevel);
            
            
            InitLevel();
            load.Close();
        }

        private void Start()
        {
            GameStartRoute();
        }

        public void GameStartRoute()
            => GameStart();

        public void GoNextLevel()
            => GameStart(currentLevel + 1);

        public void ResetLevel()
            => GameStart(1);

        private void InitLevel()
        {
            //일단 임시 데이터로 만든다.
            _levelData = new LevelData(10 + currentLevel*2, 8);
            currentScore = new IntReactiveFluffyProperty();
            
            DeckManager.I.Init(_levelData.MaxHandCard);
            UIManager.I.ChangeView(UIType.InGame);
            isGameRunning = true;
            //DeckManager.I.StartGameAfterInit();
        }

        public void AddScore(int val)
        {
            currentScore.ChangeValueByDelta(val);
        }

        public bool EndStage()
        {
            isGameRunning = false;
            //일단 승패만 출력한다
            return currentScore.Value >= _levelData.Goal;
        }

        public void SubscribeCurrentScore(Action<int> cb)
            => currentScore?.Subscribe(cb);

        public ToolType CurrentTool => currentTool;

        public void BindTileClickedHandler(Action cb)
        {
            _tileSet.BindTileClickedHandler(cb);
        }
        
        //도구의 타입은 나중에 클래스가 될 가능성이 높다.
        public void PrepareTool(ToolType type)
        {
            currentTool = type;
            //todo : 여기서 마우스 아이콘을 바꾸자.
        }
    }
}

