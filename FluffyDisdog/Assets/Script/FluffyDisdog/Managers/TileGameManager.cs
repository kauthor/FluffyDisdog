
using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using FluffyDisdog.Data.RelicData;
using FluffyDisdog.Manager;
using FluffyDisdog.UI;
using Script.FluffyDisdog.Managers;
using Script.FluffyDisdog.TileClass;
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

    public class TileGameManager:SceneRegardableSingleton<TileGameManager>
    {
        [SerializeField] private TileSet _tileSet;
#if UNITY_EDITOR
        [SerializeField] private bool _debug=false;
        [SerializeField] private RelicName[] startRelicNames;
#endif
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
        
        private TileScoreEmulator scoreEmulator;
        public TileScoreEmulator ScoreEmulator => scoreEmulator;

        [Button]
        private async void GameStart(int level=1)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif
            await UniTask.WaitUntil(() => ExcelManager.I.Initialized);
            await UniTask.WaitUntil(() => GameManager.I.Initialized);
            PlayerManager.I.Init();
            relicSystem.InitStageRelic();
            scoreEmulator=new TileScoreEmulator();
            
            var load = UILoadingPopup.NormalLoadStart();
            currentLevel = level;
            
            //임의로 최대레벨 설정
            if (currentLevel > 8)
                currentLevel = 1;
            await _tileSet.InitGame(currentLevel);
            
            
            InitLevel(_levelData, level);
            load.Close();
        }

        private void Start()
        {
            GameStartRoute();
        }

        public void GameStartRoute()
        {
            _requestSystem = new RequestSystem();
            _requestSystem.Init();

            relicSystem = new RelicSystem(PlayerManager.I);
#if UNITY_EDITOR
            if (_debug)
            {
                if(startRelicNames!=null)
                    foreach (var relicName in startRelicNames)
                    {
                        relicSystem.GainRelic(relicName);
                    }
            }
#endif
            GameStart();
        }

        public void GoNextLevel()
            => GameStart(currentLevel + 1);

        public void ResetLevel()
            => GameStart(1);

        private void InitLevel(LevelData before, int newLevel)
        {
            //일단 임시 데이터로 만든다.
            if(before == null)
                _levelData = new LevelData(800 , 8);
            else _levelData = new LevelData(
                before.Goal + ((newLevel-1)^2/8 + (newLevel%3==1? (newLevel/3)^3:0))*100, 8);
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


        public void EndScore()
        {
            AccountManager.I.AddGold(currentScore.Value*100);
        }

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


        public void GameOverProcess(bool clear = false)
        {
            if(!clear)
                UIGameOverResultPopup.OpenPopup();
            else
            {
                EndScore();
                UIStageRewardPopup.OpenPopup(() =>
                {
                    //TileGameManager.I.GoNextLevel();
                    UIManager.I.ChangeView(UIType.Store);
                });
            }
        }
    }
}

