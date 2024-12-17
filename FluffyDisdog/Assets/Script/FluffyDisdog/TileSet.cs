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
            for (int i = 0; i < nodes.Length; i++)
            {
                var cur = nodes[i];
                cur.InitNode(i%row,i/row, OnNodeClicked);
            }
        }

        private void OnNodeClicked(Tuple<int, int> coord)
        {
            var currentType = TileGameManager.I.CurrentTool;

            var clicked = nodes[coord.Item1 + row * coord.Item2];
            if (!clicked.ValidNode())
                return;
            
            OnNodeClickedCB?.Invoke();
            
            switch (currentType)
            {
                case ToolType.Shovel:
                    clicked.ChangeState();
                    break;
                case ToolType.Rake:
                    clicked.ChangeState();
                    var left = coord.Item1 - 1;
                    if (left >= 0)
                    {
                        var dig = Random.Range(0.0f, 1.0f);
                        if (dig >= 0.5f)
                        {
                            var leftNode = nodes[left + row * coord.Item2];
                            leftNode.ChangeState();
                        }
                    }
                    
                    var right = coord.Item1 + 1;
                    if (right < row)
                    {
                        var dig = Random.Range(0.0f, 1.0f);
                        if (dig >= 0.5f)
                        {
                            var rightNode = nodes[right + row * coord.Item2];
                            rightNode.ChangeState();
                        }
                    }

                    break;
            }
        }
    }
}