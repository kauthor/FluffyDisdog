﻿using System;
using Cysharp.Threading.Tasks;
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
            
            AsyncOperationHandle initHandle =
                Addressables.LoadAssetAsync<GameObject>($"Level{currentLevel}");
            initHandle.Completed += op =>
            {
                var res = op.Result as GameObject;
                currentLevelSet = GameObject.Instantiate(res, levelParent).GetComponent<TileLevel>();
                currentLevelSet.transform.localPosition = new Vector3(0, 0, 0);
            };
            await initHandle;
            Addressables.Release(initHandle);
            
            normalTotal = 0;
            diggedNormalTile = 0;
            nodeConditions = new int[nodes.Length];
            for (int i = 0; i < nodes.Length; i++)
            {
                var cur = nodes[i];
                cur.InitNode(i%row,i/row, OnNodeClicked, this);
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
            var target = Random.Range(0, len);
            while (nodeConditions[target] >=1)
            {
                target = Random.Range(0, len);
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
                var target = Random.Range(0, len);
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
                var target = Random.Range(0, len);
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

        private void OnNodeClicked(Tuple<int, int> coord)
        {
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
                    if(currentNode.ValidNode())
                        currentNode.TryDigThisBlock(data, data.GetRatioValue(j,i));
                }
            }
            
            /*switch (currentType)
            {
                case ToolType.Shovel:
                    clicked.TryDigThisBlock();
                    break;
                case ToolType.Rake:
                    clicked.TryDigThisBlock();
                    var left = coord.Item1 - 1;
                    if (left >= 0)
                    {
                        var dig = Random.Range(0.0f, 1.0f);
                        if (dig >= 0.5f)
                        {
                            var leftNode = nodes[left + row * coord.Item2];
                            if(leftNode.ValidNode())
                               leftNode.TryDigThisBlock();
                        }
                    }
                    
                    var right = coord.Item1 + 1;
                    if (right < row)
                    {
                        var dig = Random.Range(0.0f, 1.0f);
                        if (dig >= 0.5f)
                        {
                            var rightNode = nodes[right + row * coord.Item2];
                            if(rightNode.ValidNode())
                                rightNode.TryDigThisBlock();
                        }
                    }

                    break;
            }*/
            
            OnNodeClickedCB?.Invoke();
        }
    }
}