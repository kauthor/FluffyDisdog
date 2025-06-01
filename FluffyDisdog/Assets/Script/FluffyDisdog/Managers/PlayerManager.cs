using System;
using FluffyDisdog;
using UnityEngine;

namespace Script.FluffyDisdog.Managers
{
    public class RuntimeStat
    {
        private float scoreMultiplier;
        private float tileSuccessRateAdd;
        
        public float ScoreMultiplier => scoreMultiplier;
        public float TileSuccessRateAdd => tileSuccessRateAdd;

        public RuntimeStat(float scoreMultiplier, float tileSuccessRateAdd)
        {
            this.scoreMultiplier = 1+scoreMultiplier;
            this.tileSuccessRateAdd = tileSuccessRateAdd;
        }
        
        public void AddScoreMulti(float value)=> scoreMultiplier+=value;
        public void AddTileSuccessRate(float value)=> tileSuccessRateAdd+=value;
    }
    
    
    public class PlayerManager:CustomSingleton<PlayerManager>
    {
        [SerializeField] private DeckManager deckManager;
        [SerializeField] private TileGameManager tileGameManager;
        
        private TurnEventSystem turnEventSystem;
        
        public TurnEventSystem TurnEventSystem => turnEventSystem;
        private RelicSystem relicSystem;
        public RelicSystem RelicSystem => relicSystem;
        
        private RuntimeStat runtimeStat;
        public RuntimeStat RuntimeStat => runtimeStat;

        private void Start()
        {
            //Init();
        }

        public void Init()
        {
            turnEventSystem = new TurnEventSystem();
            turnEventSystem.Init();

            relicSystem = new RelicSystem(this);

            runtimeStat = new RuntimeStat(0, 0);
        }
    }
}