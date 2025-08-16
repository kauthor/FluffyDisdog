using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using FluffyDisdog.CardOptionExecuter;
using FluffyDisdog.RelicCommandData;
using Script.FluffyDisdog.Managers;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using Random = UnityEngine.Random;


namespace FluffyDisdog
{
    public class TileSet:MonoBehaviour
    {
        private Transform nodesParent => transform;

        

        [SerializeField] private Transform levelParent;
        [SerializeField] private TerrainNode tilePrefab;
        [SerializeField] private int initialRow=8;
        [SerializeField] private int initialColume=8;
        [SerializeField] private int testSeed=-1;
        private TerrainNode[] nodes => currentLevelSet.Nodes;
        private int row => currentLevelSet.Row;

        private int currentLevel = 0;
        private TileLevel currentLevelSet;
        private event Action OnNodeClickedCB;

        /// <summary>
        /// -1 : 꺼진 일반타일.
        /// 0 : 켜진 일반타일
        /// 1~5 : 장애물타일
        /// 6~10 : 보물타일
        /// </summary>
        private int[] nodeConditions;

        private int normalTotal;
        private int diggedNormalTile;

        private TurnEventSystem _eventSystem;
        public TurnEventSystem EventSystem=>_eventSystem;

        public void BindTileClickedHandler(Action cb)
        {
            OnNodeClickedCB -= cb;
            OnNodeClickedCB += cb;
        }
        
        private void Awake()
        {
            //nodes = nodesParent.GetComponentsInChildren<TerrainNode>();
            OnNodeClickedCB = null;
        }

        public async UniTask InitGame(int level =1)
        {
            if (levelParent.childCount>0)
            {
                Destroy(levelParent.GetChild(0).gameObject);
            }
            currentLevel = level;

            int seed = SeedManager.I.UpdateStage(level);


            var rand = new System.Random(seed);
            
            int obstacleAmount = 5;
            int treasureAmount = 3;

            var tileLevel = new GameObject();
            var levelSet = tileLevel.AddComponent<TileLevel>();
            tileLevel.transform.SetParent(levelParent);
            currentLevelSet = levelSet;
            currentLevelSet.transform.localPosition = new Vector3(4, 0, 0);

            TerrainNode[] tileArray = new TerrainNode[initialColume*initialColume];

            for (int i = 0; i < initialColume; i++)
            {
                for (int j = 0; j < initialRow; j++)
                {
                    var newtile = GameObject.Instantiate(tilePrefab, currentLevelSet.transform);
                    newtile.transform.localPosition = new Vector3(-4 + j*0.93f, 4 - i*0.93f, 0);
                    tileArray[j + i * initialRow] = newtile;
                }
            }

            int[] obsTargets = new int[obstacleAmount];
            for (int i = 0; i < obsTargets.Length; i++)
            {
                obsTargets[i] = -1;
            }
            int[] treasureTargets = new int[treasureAmount];
            for (int i = 0; i < treasureTargets.Length; i++)
            {
                treasureTargets[i] = -1;
            }

            int obsSuccess = 0;
            while (obsSuccess < obstacleAmount)
            {
                int nextRand = rand.Next(0, 10000) % (initialRow * initialColume);
                if(obsTargets.Contains(nextRand))
                    continue;

                obsTargets[obsSuccess++] = nextRand;
                tileArray[nextRand].RuntimePropertyInit(NodeType.Obstacle, (ObstacleType)rand.Next(0,5), TreasureType.None);
            }
            
            int treSuccess = 0;
            while (treSuccess < treasureAmount)
            {
                int nextRand = rand.Next(0, 10000) % (initialRow * initialColume);
                if(treasureTargets.Contains(nextRand) || obsTargets.Contains(nextRand))
                    continue;

                treasureTargets[treSuccess++] = nextRand;
                tileArray[nextRand].RuntimePropertyInit(NodeType.Treasure, ObstacleType.None, (TreasureType)rand.Next(0,5));
            }
            
            currentLevelSet.InitFromRuntime(initialRow, initialColume, tileArray);
            
            /*AsyncOperationHandle initHandle =
                Addressables.LoadAssetAsync<GameObject>($"Level{currentLevel}");
            initHandle.Completed += op =>
            {
                var res = op.Result as GameObject;
                currentLevelSet = GameObject.Instantiate(res, levelParent).GetComponent<TileLevel>();
                currentLevelSet.transform.localPosition = new Vector3(4, 0, 0);
            };
            await initHandle;
            Addressables.Release(initHandle);*/
            
            normalTotal = 0;
            diggedNormalTile = 0;
            nodeConditions = new int[nodes.Length];
            mouseEffectedNode = new List<TerrainNode>();
            _eventSystem = new TurnEventSystem();
            _eventSystem.Init();
            
            for (int i = 0; i < nodes.Length; i++)
            {
                var cur = nodes[i];
                cur.InitNode(i%row,i/row, OnNodeClicked, this);
                cur.InitMouseOverHandler(OnNodeMouseOver, OnNodeMouseExit);
                int blockOp = 0;
                switch (cur.blockType)
                {
                    case NodeType.NONE:
                        blockOp = 0;
                        break;
                    case NodeType.Obstacle:
                        switch (cur.ObstacleType)
                        {
                            case ObstacleType.Type2:
                                blockOp = 2;
                                break;
                            case ObstacleType.Type3:
                                blockOp = 3;
                                break;
                            case ObstacleType.Type4:
                                blockOp = 4;
                                break;
                            case ObstacleType.Type5:
                                blockOp = 5;
                                break;
                            case ObstacleType.Type1:
                            default:
                                blockOp = 1;
                                break;
                        }
                        break;
                    case NodeType.Treasure:
                        switch (cur.TreasureType)
                        {
                            case TreasureType.Type2:
                                blockOp = 7;
                                break;
                            case TreasureType.Type3:
                                blockOp = 8;
                                break;
                            case TreasureType.Type4:
                                blockOp = 9;
                                break;
                            case TreasureType.Type5:
                                blockOp = 10;
                                break;
                            case TreasureType.Type1:
                            default:
                                blockOp = 6;
                                break;
                        }
                        break;
                }
                
                nodeConditions[i] = blockOp;
                if (cur.blockType == NodeType.NONE)
                    normalTotal++;
            }

            if (normalTotal <= 0)
                normalTotal = 1;
        }

        public void RegenRandomNormalTileAsObstacle()
        {
            var len = nodeConditions.Length;
            var target = UnityEngine.Random.Range(0, len);
            while (nodeConditions[target] >=1)
            {
                target = UnityEngine.Random.Range(0, len);
            }

            nodeConditions[target] = 1;
            nodes[target].RegenerateAsObstacle();
        }

        public void SwapNormalTiles()
        {
            var len = nodeConditions.Length;
            for (int i = 0; i < len; i++)
            {
                var current = nodeConditions[i];
                if(nodeConditions[i] >= 1)
                    continue;
                var target = UnityEngine.Random.Range(0, len);
                var targetCondition = nodeConditions[target];
                if(targetCondition >=1 || targetCondition == current)
                    continue;

                nodeConditions[i] = targetCondition;
                nodeConditions[target] = current;
            }

            for (int i = 0; i < len; i++)
            {
                var current = nodeConditions[i];
                if (current < 1)
                {
                    nodes[i].EnableNode(current==0);
                }
            }
        }

        public void SwapAllTiles()
        {
            var len = nodeConditions.Length;
            for (int i = 0; i < len; i++)
            {
                var current = nodeConditions[i];
                var target = UnityEngine.Random.Range(0, len);
                var targetCondition = nodeConditions[target];
                if(targetCondition == current)
                    continue;

                nodeConditions[i] = targetCondition;
                nodeConditions[target] = current;
            }

            for (int i = 0; i < len; i++)
            {
                var current = nodeConditions[i];
                nodes[i].SwapNodeByData(current);
            }
        }

        public void TryAddExecutedNode(TerrainNode node)
        {
            var coord = node.Coord;
            var num =coord.Item1 + row * coord.Item2;
            nodeConditions[num] = -1;
            TileGameManager.I.AddScore(1);
        }

        private List<TerrainNode> mouseEffectedNode;

        private void OnNodeMouseOver(Tuple<int, int> coord)
        {
            if (mouseEffectedNode.Count != 0)
                mouseEffectedNode.ForEach(_ => _.MouseOverOnOff(false));
            
            mouseEffectedNode.Clear();
            var currentType = TileGameManager.I.CurrentTool;
            if (currentType == ToolType.None)
                return;

            var clicked = nodes[coord.Item1 + row * coord.Item2];
            if (!clicked.ValidNode())
                return;

            //이것도 추후 타일처럼 디자인패턴화 시키자...

            var data = ExcelManager.I.GetToolData(currentType);
            int startCoordCol = coord.Item2 - data.CenterColumn;
            int startCoordRow = coord.Item1 - data.CenterRow;

            for (int i = 0; i < data.cellHeight; i++)
            {
                int currentH = i + startCoordCol;
                if(currentH<0 || currentH >= currentLevelSet.Column)
                    continue;
                for (int j = 0; j < data.cellWidth; j++)
                {
                    int currentW = j + startCoordRow;
                    if(currentW < 0 || currentW >= currentLevelSet.Row)
                        continue;
                    
                    var currentNode = nodes[currentW + row * currentH];
                    //여기서 활성화여부 체크
                    if(currentNode.ValidNode())
                    {
                        currentNode.MouseOverOnOff(true);
                        mouseEffectedNode.Add(currentNode);
                    }
                }
            }
        }

        private void OnNodeMouseExit(Tuple<int, int> coord)
        {
            if (mouseEffectedNode.Count != 0)
                mouseEffectedNode.ForEach(_ => _.MouseOverOnOff(false));
            
            mouseEffectedNode.Clear();
        }

        public List<TerrainNode> GetTilesByRange(Tuple<int, int> coord, int range)
        {
            var center = nodes[coord.Item1 + row * coord.Item2];
            var ret = new List<TerrainNode>();
            int startCoordCol = coord.Item2 - range;
            int startCoordRow = coord.Item1 - range;
            for (int i = 0; i < range*2+1; i++)
            {
                int currentH = i + startCoordCol;
                if(currentH<0 || currentH >= currentLevelSet.Column)
                    continue;
                for (int j = 0; j < range*2+1; j++)
                {
                    int currentW = j + startCoordRow;
                    if(currentW < 0 || currentW >= currentLevelSet.Row)
                        continue;
                    
                    var currentNode = nodes[currentW + row * currentH];
                    //여기서 활성화여부 체크
                    ret.Add(currentNode);
                }
            }

            return ret;
        }

        public List<TerrainNode> GetNearTiles(Tuple<int, int> coord)
        {
            //0북 1서 2남 3동
            var ret = new List<TerrainNode>();
            for (int i = 0; i < 2; i++)
            {
                int currentH = coord.Item2 + (i == 0 ? -1 : 1);
                if(currentH<0 || currentH >= currentLevelSet.Column)
                    continue;
                var currentNode = nodes[coord.Item1 + row * currentH];
                ret.Add(currentNode);
            }
            for (int i = 0; i < 2; i++)
            {
                int currentW = coord.Item1 + (i == 0 ? -1 : 1);
                if(currentW<0 || currentW >= currentLevelSet.Row)
                    continue;
                var currentNode = nodes[currentW + row * coord.Item2];
                ret.Add(currentNode);
            }

            return ret;
        }
        
        
        private void OnNodeClicked(Tuple<int, int> coord)
        {
            var currentType = TileGameManager.I.CurrentTool;
            if (currentType == ToolType.None)
                return;

            var clicked = nodes[coord.Item1 + row * coord.Item2];
            if (!clicked.ValidNode())
                return;

            var param = new CardExecuteParam(clicked, 0);
            var ex = DeckManager.I.CurrentCard.Executor;
            
            ex.PreEffect(param);
            
            //이것도 추후 타일처럼 디자인패턴화 시키자...
            var beforeScore = TileGameManager.I.CurrentScore.Value;
            PlayerManager.I.TurnEventSystem.FireEvent(TurnEvent.TileClicked, new TileClickedParam()
            {
                targetNode = clicked
            });

            var data = ExcelManager.I.GetToolData(currentType);
            int startCoordCol = coord.Item2 - data.CenterColumn;
            int startCoordRow = coord.Item1 - data.CenterRow;

            float preEndParamOut = param.output;
            
            int nodeCracked = 0;
            for (int i = 0; i < data.cellHeight; i++)
            {
                int currentH = i + startCoordCol;
                if(currentH<0 || currentH >= currentLevelSet.Column)
                    continue;
                for (int j = 0; j < data.cellWidth; j++)
                {
                    int currentW = j + startCoordRow;
                    if(currentW < 0 || currentW >= currentLevelSet.Row)
                        continue;
                    
                    float addedRate = PlayerManager.I.RuntimeStat.ScoreMultiplier;
                    
                    var calParam = new ToolCalculateStart()
                    {
                        toolType = currentType,
                    };
                    PlayerManager.I.TurnEventSystem.FireEvent(TurnEvent.ToolCalculateStart, calParam);
                    
                    addedRate+= calParam.addRate;
                    
                    var currentNode = nodes[currentW + row * currentH];

                    var tileParam = new CardExecuteParam(currentNode, preEndParamOut); 
                    ex.ExecuteTileEffect(tileParam);
                    preEndParamOut = param.output;
                    
                    if (currentNode.Coord.Item1 == coord.Item1 || currentNode.Coord.Item2 == coord.Item2)
                    {
                        var hparam = new CrackPointMeasureParam()
                        {
                            clicked = coord,
                            target = currentNode.Coord
                        };
                        PlayerManager.I.TurnEventSystem.FireEvent(TurnEvent.DistanceDesire, hparam);
                        
                        addedRate += hparam.addedRate;
                    }
                    
                    //여기서 활성화여부 체크
                    if (currentNode.ValidNode())
                    {
                        if (data.GetInteractable(j, i))
                        {
                            if (currentNode.TryDigThisBlock(data, data.GetRatioValue(j, i) + (int)(addedRate*100.0f)))
                            {
                                nodeCracked++;
                            }
                            else
                            {
                                PlayerManager.I.TurnEventSystem.FireEvent(TurnEvent.DigFail, new DigFailParam()
                                {
                                    target = currentNode,
                                });
                            }
                        }
                    }
                }
            }
            
            var afterScore = TileGameManager.I.CurrentScore.Value;
            var endParam = new CardExecuteParam(clicked, preEndParamOut);
            
            ex.PostEffect(endParam);
            
            float ret = param.output;
            
            if(nodeCracked > 0)
                PlayerManager.I.TurnEventSystem.FireEvent(TurnEvent.EndCrack, new OnEndCrackParam()
                {
                    digged = nodeCracked,
                });
            
            //todo : 여기에 OnEndCrackParam 으로 점수배율 책정 후 덧셈.
            
            
            
            if (mouseEffectedNode.Count != 0)
                mouseEffectedNode.ForEach(_ => _.MouseOverOnOff(false));
            
            mouseEffectedNode.Clear();

            OnNodeClickedCB?.Invoke();
            _eventSystem?.FireEvent(TurnEvent.TurnEnd);
            _eventSystem?.FireEvent(TurnEvent.TurnStart);
        }


        public TerrainNode GetRandomNode(Func<TerrainNode, bool> predicate = null)
        {
            if (predicate == null)
                return null;
            var rand = nodes[Random.Range(0, nodes.Length)];
            while (!predicate(rand))
            {
                rand = nodes[Random.Range(0, nodes.Length)];
            }

            return rand;
        }
    }
}