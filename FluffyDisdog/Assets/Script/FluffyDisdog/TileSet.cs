using System;
using UnityEngine;
using Random = UnityEngine.Random;


namespace FluffyDisdog
{
    public class TileSet:MonoBehaviour
    {
        private Transform nodesParent => transform;

        private TerrainNode[] nodes;
        [SerializeField] private int row;
        [SerializeField] private int column;

        private event Action OnNodeClickedCB;

        /// <summary>
        /// -1 : 꺼진 일반타일.
        /// 0 : 켜진 일반타일
        /// 1 : 장애물타일
        /// 2 : 보물타일
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
            nodes = nodesParent.GetComponentsInChildren<TerrainNode>();
            OnNodeClickedCB = null;
            if (row <= 0)
                row = 1;
        }

        public void InitGame()
        {
            normalTotal = 0;
            diggedNormalTile = 0;
            nodeConditions = new int[nodes.Length];
            for (int i = 0; i < nodes.Length; i++)
            {
                var cur = nodes[i];
                cur.InitNode(i%row,i/row, OnNodeClicked, this);
                nodeConditions[i] = (int) cur.blockType;
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

        public void TryAddExecutedNode(TerrainNode node)
        {
            var coord = node.Coord;
            var num =coord.Item1 + row * coord.Item2;
            nodeConditions[num] = -1;
        }

        private void OnNodeClicked(Tuple<int, int> coord)
        {
            var currentType = TileGameManager.I.CurrentTool;

            var clicked = nodes[coord.Item1 + row * coord.Item2];
            if (!clicked.ValidNode())
                return;
            
            OnNodeClickedCB?.Invoke();
            
            //이것도 추후 타일처럼 디자인패턴화 시키자...
            
            switch (currentType)
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
            }
        }
    }
}