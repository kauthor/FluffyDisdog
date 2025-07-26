
using System;
using FluffyDisdog.Manager;
using FluffyDisdog.UI;
using Script.FluffyDisdog.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FluffyDisdog
{
    

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
        private RequestSystem _requestSystem;
        public RequestSystem RequestSystem => _requestSystem;

        private ToolType currentTool = ToolType.None;
        private int currentId = 0;

        private LevelData _levelData;
        public LevelData LevelData => _levelData;
        private IntReactiveFluffyProperty currentScore;
        public IntReactiveFluffyProperty CurrentScore => currentScore;
        private bool isGameRunning=false;

        public bool IsGameRunning => isGameRunning;
        
        private RelicSystem relicSystem;
        public RelicSystem RelicSystem => relicSystem;

        public int currentLevel = 1;

        [Button]
        private async void GameStart(int level=1)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif
            PlayerManager.I.Init();
            relicSystem.InitStageRelic();
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
            _requestSystem = new RequestSystem();
            _requestSystem.Init();

            relicSystem = new RelicSystem(PlayerManager.I);
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
            PlayerManager.I.TurnEventSystem.FireEvent(TurnEvent.GameStart);
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
        public void PrepareTool(ToolType type, int id)
        {
            currentTool = type;
            currentId = id;
            //todo : 여기서 마우스 아이콘을 바꾸자.
        }
    }
}

