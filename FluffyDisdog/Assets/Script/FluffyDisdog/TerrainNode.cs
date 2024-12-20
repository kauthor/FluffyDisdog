﻿using System;
using Script.FluffyDisdog.TileClass;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FluffyDisdog
{
    public enum NodeState
    {
        Raw=0,
        Digged=1,
    }

    public enum NodeType
    {
        NONE=0,
        Obstacle=1,
        Treasure=2
    }

    public enum ObstacleType
    {
        None=-1,
        Type1=0,
        Type2=1,
        Type3=2,
        Type4=3,
        Type5=4
    }
    
    public enum TreasureType
    {
        None=-1,
        Type1=0,
        Type2=1,
        Type3=2,
        Type4=3,
        Type5=4
    }

    public abstract class NodeExecuter
    {
        public abstract void Execute();

        protected TileSet parentTileSet;
        protected TerrainNode node;
        private void SetData(TileSet set, TerrainNode n)
        {
            parentTileSet = set;
            node = n;
        }

        public static NodeExecuter MakeExecuter(TerrainNode node, TileSet tileSet)
        {
            NodeExecuter ret;
            switch (node.blockType)
            {
                case NodeType.Obstacle:
                    switch (node.ObstacleType)
                    {
                        case ObstacleType.Type2:
                            ret = new ObstacleType2();
                            break;
                        case ObstacleType.Type3:
                            ret = new ObstacleType3();
                            break;
                        case ObstacleType.Type4:
                            ret = new ObstacleType4();
                            break;
                        case ObstacleType.Type5:
                            ret = new ObstacleType5();
                            break;
                        case ObstacleType.Type1:
                        default:
                            ret = new ObstacleType1();
                            break;
                    }
                    break;
                case NodeType.Treasure:
                    switch (node.TreasureType)
                    {
                        case TreasureType.Type2:
                            ret = new TreasureType2();
                            break;
                        case TreasureType.Type3:
                            ret = new TreasureType3();
                            break;
                        case TreasureType.Type4:
                            ret = new TreasureType4();
                            break;
                        case TreasureType.Type5:
                            ret = new TreasureType5();
                            break;
                        case TreasureType.Type1:
                        default:
                            ret = new TreasureType1();
                            break;
                    }

                    break;
                case NodeType.NONE:
                default:
                    ret = new NormalTile();
                    break;
            }

            ret?.SetData(tileSet, node);
            return ret;
        }
    }

    public class TerrainNode:MonoBehaviour
    {
        public bool isObstacle => blockType == NodeType.Obstacle;
        private Tuple<int, int> coord;
        public Tuple<int, int> Coord => coord;
        private Action<Tuple<int, int>> onClicked;
        [SerializeField]private SpriteRenderer _renderer;

        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Sprite[] treasurePool;
        [SerializeField] private Sprite[] obstaclePool;

        private NodeState currentState;
        

        [FoldoutGroup("Block Property")] [SerializeField]
        public NodeType blockType;

        [FoldoutGroup("Block Property")]
        [ShowIf(nameof(isTreasure))]
        [SerializeField] private TreasureType treasureType= TreasureType.None;

        public TreasureType TreasureType => treasureType;
        
        [FoldoutGroup("Block Property")]
        [ShowIf(nameof(isObstacle))]
        [SerializeField] private ObstacleType obstacleType=ObstacleType.None;

        public ObstacleType ObstacleType => obstacleType;

        public bool isTreasure => blockType == NodeType.Treasure;

        private TileSet parent;
        private NodeExecuter Executer;

        public void InitNode(int row, int col, Action<Tuple<int, int>> cb, TileSet par)
        {
            coord = new Tuple<int, int>(row, col);
            parent = par;
            onClicked = cb;
            currentState = NodeState.Raw;
            UpdateSprite();
            Executer = NodeExecuter.MakeExecuter(this, parent);
        }

        [Button]
        private void UpdateSprite()
        {
            switch (blockType)
            {
                case NodeType.NONE:
                    _renderer.sprite = normalSprite;
                    break;
                case NodeType.Treasure:
                    _renderer.sprite = treasurePool[(int) treasureType];
                    break;
                case NodeType.Obstacle:
                    _renderer.sprite = obstaclePool[(int) obstacleType];
                    break;
            }
        }

        private void OnMouseDown()
        {
            onClicked?.Invoke(coord);
        }

        public void TryDigThisBlock()
        {
            if (currentState == NodeState.Digged)
                return;
            
            Executer?.Execute();

            //여기에서 게임 매니저에 점수 호출
        }
        

        public void EnableNode(bool alive)
        {
            if (alive)
            {
                currentState = NodeState.Raw;
                UpdateSprite();
            }
            else
            {
                currentState = NodeState.Digged;
                _renderer.sprite = null;
            }
        }

        public void RegenerateAsObstacle()
        {
            blockType = NodeType.Obstacle;
            obstacleType = ObstacleType.Type1;
            EnableNode(true);
            Executer = NodeExecuter.MakeExecuter(this, parent);
        }

        public bool ValidNode() => currentState == NodeState.Raw;
    }
}