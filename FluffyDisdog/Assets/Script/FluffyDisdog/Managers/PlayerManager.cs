using System;
using System.Collections.Generic;
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
    
    
    public class PlayerManager:SceneRegardableSingleton<PlayerManager>
    {
        [SerializeField] private DeckManager deckManager;
        [SerializeField] private TileGameManager tileGameManager;

        private List<int> tagBookmark;
        private List<NodeSubstate> nodeSubstateBookmark;
        
        public List<int> TagBookmark => tagBookmark;
        public List<NodeSubstate> NodeSubstateBookmark => nodeSubstateBookmark;
        
        private TurnEventSystem turnEventSystem;
        
        public TurnEventSystem TurnEventSystem => turnEventSystem;
        
        
        private RuntimeStat runtimeStat;
        public RuntimeStat RuntimeStat => runtimeStat;

        private void Start()
        {
            //Init();
            this.tagBookmark = new List<int>();
            this.nodeSubstateBookmark = new List<NodeSubstate>();
        }

        public void TagBookmarkInsert(bool regist = true, int tag = 1)
        {
            if(regist && !tagBookmark.Contains(tag))
                this.tagBookmark.Insert(0,tag);
            else if(!regist && tagBookmark.Contains(tag))
                this.tagBookmark.Remove(tag);
        }

        public void NodeSubstateBookmarkInsert(bool regist = true, NodeSubstate tag = NodeSubstate.NONE)
        {
            if(regist && !nodeSubstateBookmark.Contains(tag))
                this.nodeSubstateBookmark.Insert(0,tag);
            else if(!regist && nodeSubstateBookmark.Contains(tag))
                this.nodeSubstateBookmark.Remove(tag);
        }

        public void Init()
        {
            turnEventSystem = new TurnEventSystem();
            turnEventSystem.Init();

            runtimeStat = new RuntimeStat(0, 0);
        }
    }
}