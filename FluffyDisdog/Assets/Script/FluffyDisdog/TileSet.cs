using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using FluffyDisdog.CardOptionExecuter;
using FluffyDisdog.RelicCommandData;
using FluffyDisdog.UI;
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
        [SerializeField] private Transform damageParent;
        [SerializeField] private Animation shakeAnim;
        
        [SerializeField] private GameObject treasurePrefab;
        
        [SerializeField] private Transform treasureParent;
        private Stack<GameObject> treasurePool = new Stack<GameObject>();
        private Stack<GameObject> currentTreasure = new Stack<GameObject>();
        private TerrainNode[] nodes => currentLevelSet.Nodes;
        private int row => currentLevelSet.Row;

        private int currentLevel = 0;
        private TileLevel currentLevelSet;
        public TileLevel CurrentLevelSet => currentLevelSet;
        private event Action OnNodeClickedCB;

        private int validNodeCount = 0;
        public int ValidNodeCount => validNodeCount;

        private TreasureSystem _treasureSystem;
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

        [SerializeField] private DamageFontPart fontPartPrefab;
        [SerializeField] private GameObject hitPrefab;
        [SerializeField] private GameObject hitfailPrefab; 
        private Stack<DamageFontPart> fontPool;

        public void BindTileClickedHandler(Action cb)
        {
            OnNodeClickedCB -= cb;
            OnNodeClickedCB += cb;
        }
        
        private void Awake()
        {
            //nodes = nodesParent.GetComponentsInChildren<TerrainNode>();
            
            fontPool = new Stack<DamageFontPart>();
            _treasureSystem = new TreasureSystem();
        }

        public async UniTask InitGame(int level =1)
        {
            OnNodeClickedCB = null;
            if (levelParent.childCount>0)
            {
                Destroy(levelParent.GetChild(0).gameObject);
            }
            currentLevel = level;

            int seed = SeedManager.I.UpdateStage(level);


            var rand = new System.Random(seed);
            
            int obstacleAmount = 5 + level/2;
            int treasureAmount = 3;

            var tileLevel = new GameObject();
            var levelSet = tileLevel.AddComponent<TileLevel>();
            tileLevel.transform.SetParent(levelParent);
            currentLevelSet = levelSet;
            currentLevelSet.transform.localScale = new Vector3(1,1,1);
            currentLevelSet.transform.localPosition = new Vector3(0, 0, 0);

            TerrainNode[] tileArray = new TerrainNode[initialColume*initialColume];

            for (int i = 0; i < initialColume; i++)
            {
                for (int j = 0; j < initialRow; j++)
                {
                    var newtile = GameObject.Instantiate(tilePrefab, currentLevelSet.transform);
                    newtile.transform.localPosition = new Vector3(-86.7f + j*25, 86.5f - i*25, 0);
                    tileArray[j + i * initialRow] = newtile;
                }
            }
            
            //todo : 보물을 생성한다. 일단은... 4개까지 나올 떄 최적화 된 방식으로다가....
            
            _treasureSystem.ClearAndInit();
            int hiddenSuccess = 0;
            bool[] quadInit=new bool[4] {false,false,false,false};

            if (currentTreasure.Count > 0)
            {
                for (; currentTreasure.Count > 0;)
                {
                    var current = currentTreasure.Pop();
                    treasurePool.Push(current);
                }
            }

            int trMax = 0;
            List<int> treasureCoord=new List<int>();
            while (hiddenSuccess < trMax)
            {
                int quadDiv = Random.Range(0, 4); 
                if(quadInit[quadDiv]) continue;

                hiddenSuccess++;
                quadInit[quadDiv]=true;
                
                int xCoord = rand.Next(0, 10000) % ( (initialRow-3) /2) + (quadDiv%2==0? 0: (initialRow /2));
                int yCoord = rand.Next(0, 10000) % ( (initialColume-3) /2) + (quadDiv/2==0? 0: (initialColume /2));
                _treasureSystem.TryGenerateTreasure(3,2,1, xCoord,yCoord,this);
                for (int j = 0; j < 2; j++)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        treasureCoord.Add((j+yCoord)*row + i + xCoord);
                    }
                }

                GameObject newTr;
                if (treasurePool.Count > 0)
                {
                    newTr = treasurePool.Pop();
                }
                else
                {
                    newTr = GameObject.Instantiate(treasurePrefab,treasureParent.transform);
                }
                currentTreasure.Push(newTr);
                newTr.transform.localPosition = new Vector3(-86.7f + (xCoord+1.5f)*25, 86.5f - (yCoord+1)*25, 0);
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
                if(obsTargets.Contains(nextRand) || treasureCoord.Contains(nextRand))
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
            
            SoundManager.I.PlayBgm(SoundDesc.InGame1Bgm);
            SoundManager.I.PlayEnv(SoundDesc.Env1);
            
            UpdateValidNodeCount();
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

        /// <summary>
        /// -1 : 꺼진 일반타일.
        /// 0 : 켜진 일반타일
        /// 1~5 : 장애물타일
        /// 6~10 : 보물타일
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int GetNodeCondition(int x, int y)
        {
            return nodeConditions[row*y +x];
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
            
            var hit = GameObject.Instantiate(hitPrefab, damageParent);
            hit.transform.position = node.transform.position;
            
            TileGameManager.I.GameLog.DestroyTile();
        }
        

        public void ShowAndGainScore(TileEmulatorOptionParam param, TerrainNode target)
        {
            var score = (int)(100 * (PlayerManager.I.RuntimeStat.ScoreMultiplier + param.addedScoreMulti));

            score += PlayerManager.I.RuntimeStat.ScoreAdd + param.addedScoreAbs;
            DamageFontPart parameter=null;

            if (fontPool.Count > 0)
            {
                parameter = fontPool.Pop();
                if(parameter==null)
                    parameter = GameObject.Instantiate(fontPartPrefab, damageParent);
            }
            else
            {
                parameter = GameObject.Instantiate(fontPartPrefab, damageParent);
            }
            
            parameter.gameObject.SetActive(true);
            parameter.Show(score, target.transform, _ =>
            {
                _.gameObject.SetActive(false);
                fontPool.Push(_);
            }).Forget();
            
            TileGameManager.I.AddScore(score);
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
                    if(!data.interact[j+i*data.cellWidth])
                        continue;
                    var currentNode = nodes[currentW + row * currentH];
                    //여기서 활성화여부 체크
                    if(currentNode.ValidNode())
                    {
                        currentNode.MouseOverOnOff(true, data.ratio[j+i*data.cellWidth]);
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
            //내가 다 헷갈릴 수 있으니... 일단 주석을 단다.
            var currentType = TileGameManager.I.CurrentTool;
            if (currentType == ToolType.None)
                return;

            //우선 여기서 연출을 실행시킨다. 이건... 서순이 바뀌어도 무방하다.
            shakeAnim?.Play();
            var clicked = nodes[coord.Item1 + row * coord.Item2];
            if (!clicked.ValidNode())
                return;

            var param = new CardExecuteParam(clicked, 0);
            var ex = DeckManager.I.CurrentCard.Executor;
            
            if(ex != null)
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
            bool crackSubOn = false;
            int nodeSubstateCracked = 0;
            int nodeCracked = 0;
            List<TerrainNode> emulateFailed = new List<TerrainNode>();
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
                    
                    //float addedRate = PlayerManager.I.RuntimeStat.TileSuccessRateAdd;
                    
                    var currentNode = nodes[currentW + row * currentH];
                    

                    var calParam = new TileEmulatorOptionParam()
                    {
                        toolType = currentType,
                        clicked = clicked,
                        target = currentNode,
                        clickedCoord = clicked.Coord,
                        targetCoord = currentNode.Coord,
                    };
                        
                    PlayerManager.I.TurnEventSystem.FireEvent(TurnEvent.ToolCalculateStart, calParam);
                    
                    //addedRate+= calParam.addRate;
                    
                    PlayerManager.I.TurnEventSystem.FireEvent(TurnEvent.DistanceDesire, calParam);
                    
                    //여기서 활성화여부 체크
                    if (currentNode.ValidNode())
                    {
                        if (data.GetInteractable(j, i))
                        {
                            if(ex != null)
                                ex.ExecuteWhenTileTryInteract(new CardExecuteParam(currentNode,0));
                            if (currentNode.TryDigThisBlock(data, data.GetRatioValue(j, i) /*+ (int)(addedRate*100.0f)*/))
                            {
                                nodeCracked++;
                                PlayerManager.I.TurnEventSystem.FireEvent(TurnEvent.TileDigged, calParam);
                                if(ex != null)
                                    ex.ExecuteWhenTileSuccess(new CardExecuteParam(currentNode,0));
                                ShowAndGainScore(calParam, currentNode);
                            }
                            else
                            {
                                emulateFailed.Add(currentNode);
                                PlayerManager.I.TurnEventSystem.FireEvent(TurnEvent.DigFail, calParam);
                                var hitfail = GameObject.Instantiate(hitfailPrefab, damageParent);
                                hitfail.transform.position = currentNode.transform.position;
                            }
                        }
                    }

                    var tileParam = new CardExecuteParam(currentNode, preEndParamOut);
                    bool current = currentNode.SubstateSystem.Is(NodeSubstate.Crack);
                    if(ex != null) ex.ExecuteTileEffect(tileParam);
                    preEndParamOut = param.output;
                    bool after = currentNode.SubstateSystem.Is(NodeSubstate.Crack);
                    if (!current && after)
                    {
                        nodeCracked++;
                        crackSubOn = true;
                    }
                    
                    //여기서 도구 파괴여부 체크.
                    
                    
                    
                }
            }
            
            var afterScore = TileGameManager.I.CurrentScore.Value;
            var endParam = new AfterEmulateParam(clicked, emulateFailed,nodeCracked);
            
            if(ex != null) ex.PostEffect(endParam);
            
            float ret = param.output;
            
            if(nodeCracked > 0)
            {
                PlayerManager.I.TurnEventSystem.FireEvent(TurnEvent.EndCrack, new OnEndCrackParam()
                {
                    digged = nodeCracked,
                });
                SoundManager.I.PlaySfxRandom(new SoundDesc[2]
                {
                    SoundDesc.TileDestroy1Sfx, SoundDesc.TileDestroy2Sfx
                });
            }
            else if(crackSubOn)
                SoundManager.I.PlaySFX(SoundDesc.TileCrackSfx);
            else
            {
                SoundManager.I.PlaySFX(SoundDesc.TileFailSfx);
            }
            
            
            
            if (mouseEffectedNode.Count != 0)
                mouseEffectedNode.ForEach(_ => _.MouseOverOnOff(false));
            
            mouseEffectedNode.Clear();

            OnNodeClickedCB?.Invoke();
            
            PlayerManager.I.TurnEventSystem.FireEvent(TurnEvent.TurnEnd);
            PlayerManager.I.TurnEventSystem.FireEvent(TurnEvent.TurnStart);
            _eventSystem?.FireEvent(TurnEvent.TurnEnd);
            _eventSystem?.FireEvent(TurnEvent.TurnStart);
            
            UpdateValidNodeCount();
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

        public void UpdateValidNodeCount()
        {
            int valid = 0;
            foreach (var node in nodes)
            {
                if (node.ValidNode())
                    valid++;
            }
            validNodeCount = valid;
        }

        public void HitFail(TerrainNode node)
        {
           var eff=  GameObject.Instantiate(hitfailPrefab, damageParent);
           eff.transform.position = node.transform.position;
        }


        public List<TerrainNode> GetAllNodesByDirection(int direction, TerrainNode target)
        {
            var ret = new List<TerrainNode>();
            switch (direction)
            {
                case 1:
                    for (int i = 0; i < initialRow; i++)
                    {
                        ret.Add(nodes[target.Coord.Item2*initialRow + i]);
                    }
                    break;
                case 2:
                    for (int i = 0; i < initialColume; i++)
                    {
                        ret.Add(nodes[target.Coord.Item1 + initialRow*i]);
                    }

                    break;
                case 3:
                    for (int i = 0; i < initialRow; i++)
                    {
                        ret.Add(nodes[target.Coord.Item2*initialRow + i]);
                    }
                    for (int i = 0; i < initialColume; i++)
                    {
                        ret.Add(nodes[target.Coord.Item1 + initialRow*i]);
                    }

                    break;
                case 4:
                    for (int i = target.Coord.Item2 * initialRow + target.Coord.Item1;
                         i >= 0 && i < nodes.Length;
                         i -= (initialRow - 1))
                    {
                        ret.Add(nodes[i]);
                    }
                    for (int i = target.Coord.Item2 * initialRow + target.Coord.Item1;
                         i >= 0 && i < nodes.Length;
                         i += (initialRow - 1))
                    {
                        ret.Add(nodes[i]);
                    }

                    break;
                case 5:
                    for (int i = target.Coord.Item2 * initialRow + target.Coord.Item1;
                         i >= 0 && i < nodes.Length;
                         i -= (initialRow + 1))
                    {
                        ret.Add(nodes[i]);
                    }
                    for (int i = target.Coord.Item2 * initialRow + target.Coord.Item1;
                         i >= 0 && i < nodes.Length;
                         i += (initialRow + 1))
                    {
                        ret.Add(nodes[i]);
                    }

                    break;
                case 6:
                    for (int i = target.Coord.Item2 * initialRow + target.Coord.Item1;
                         i >= 0 && i < nodes.Length;
                         i -= (initialRow - 1))
                    {
                        ret.Add(nodes[i]);
                    }
                    for (int i = target.Coord.Item2 * initialRow + target.Coord.Item1;
                         i >= 0 && i < nodes.Length;
                         i += (initialRow - 1))
                    {
                        ret.Add(nodes[i]);
                    }
                    for (int i = target.Coord.Item2 * initialRow + target.Coord.Item1;
                         i >= 0 && i < nodes.Length;
                         i -= (initialRow + 1))
                    {
                        ret.Add(nodes[i]);
                    }
                    for (int i = target.Coord.Item2 * initialRow + target.Coord.Item1;
                         i >= 0 && i < nodes.Length;
                         i += (initialRow + 1))
                    {
                        ret.Add(nodes[i]);
                    }

                    break;
                case 7:
                    for (int i = target.Coord.Item2 * initialRow + target.Coord.Item1;
                         i >= 0 && i < nodes.Length;
                         i -= (initialRow - 1))
                    {
                        ret.Add(nodes[i]);
                    }
                    for (int i = target.Coord.Item2 * initialRow + target.Coord.Item1;
                         i >= 0 && i < nodes.Length;
                         i += (initialRow - 1))
                    {
                        ret.Add(nodes[i]);
                    }
                    for (int i = target.Coord.Item2 * initialRow + target.Coord.Item1;
                         i >= 0 && i < nodes.Length;
                         i -= (initialRow + 1))
                    {
                        ret.Add(nodes[i]);
                    }
                    for (int i = target.Coord.Item2 * initialRow + target.Coord.Item1;
                         i >= 0 && i < nodes.Length;
                         i += (initialRow + 1))
                    {
                        ret.Add(nodes[i]);
                    }
                    for (int i = 0; i < initialRow; i++)
                    {
                        ret.Add(nodes[target.Coord.Item2*initialRow + i]);
                    }
                    for (int i = 0; i < initialColume; i++)
                    {
                        ret.Add(nodes[target.Coord.Item1 + initialRow*i]);
                    }

                    break;
                    
            }
            return ret;
        }
    }
}