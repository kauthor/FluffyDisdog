using System;
using FluffyDisdog;
using UnityEngine;

namespace Script.FluffyDisdog.Managers
{
    public class PlayerManager:CustomSingleton<PlayerManager>
    {
        [SerializeField] private DeckManager deckManager;
        [SerializeField] private TileGameManager tileGameManager;
        
        private TurnEventSystem turnEventSystem;
        
        public TurnEventSystem TurnEventSystem => turnEventSystem;
        private RelicSystem relicSystem;
        public RelicSystem RelicSystem => relicSystem;

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            turnEventSystem = new TurnEventSystem();
            turnEventSystem.Init();

            relicSystem = new RelicSystem(this);
        }
    }
}