using System;
using Script.FluffyDisdog.TileClass;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;


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

    public struct DefaultNodeSetting
    {
        public TreasureType trType;
        public ObstacleType obType;
        public NodeType ndType;
    }

    public class TerrainNode:MonoBehaviour
    {
        private DefaultNodeSetting _defaultNodeSetting;
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

        private void Awake()
        {
            _defaultNodeSetting = new DefaultNodeSetting()
            {
                ndType = blockType,
                trType = treasureType,
                obType = obstacleType
            };
        }

        public void InitNode(int row, int col, Action<Tuple<int, int>> cb, TileSet par)
        {
            blockType = _defaultNodeSetting.ndType;
            treasureType = _defaultNodeSetting.trType;
            obstacleType = _defaultNodeSetting.obType;
            
            
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
            if (!TileGameManager.I.IsGameRunning)
                return;
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                return;
            
            onClicked?.Invoke(coord);
        }

        public void TryDigThisBlock(int rate = 100)
        {
            if (currentState == NodeState.Digged)
                return;

            if (rate <= 0)
                return;
            
            if (rate >= 100)
            {
                Executer?.Execute();
                return;
            }

            int rand = Random.Range(0, 100);
            if(rand <= rate)
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

        public void SwapNodeByData(int newType)
        {
            switch (newType)
            {
                case -1:
                    RegenerateAsNormalTile();
                    break;
                case 0:
                    RegenerateAsNormalTile(true);
                    break;
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                    RegenerateAsObstacle((ObstacleType)(newType-1));
                    break;
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                    RegenerateAsTreasure((TreasureType)(newType-6));
                    break;
            }
        }

        public void RegenerateAsNormalTile(bool enable=false)
        {
            blockType = NodeType.NONE;
            EnableNode(enable);
            Executer = NodeExecuter.MakeExecuter(this, parent);
        }
        public void RegenerateAsObstacle(ObstacleType type = ObstacleType.Type1)
        {
            blockType = NodeType.Obstacle;
            obstacleType = type;
            EnableNode(true);
            Executer = NodeExecuter.MakeExecuter(this, parent);
        }
        public void RegenerateAsTreasure(TreasureType type = TreasureType.Type1)
        {
            blockType = NodeType.Treasure;
            treasureType = type;
            EnableNode(true);
            Executer = NodeExecuter.MakeExecuter(this, parent);
        }

        public bool ValidNode() => currentState == NodeState.Raw;
    }
}