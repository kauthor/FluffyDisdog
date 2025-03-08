using System;
using UnityEngine;

namespace FluffyDisdog
{
    public class TileLevel:MonoBehaviour
    {
        private TerrainNode[] nodes;
        [SerializeField] private int row;
        [SerializeField] private int column;


        public TerrainNode[] Nodes => nodes;
        public int Row => row;
        public int Column => column;
        private void Awake()
        {
            if(transform.childCount>0)
                nodes = GetComponentsInChildren<TerrainNode>();
        }

        public void InitFromRuntime(int r, int c, TerrainNode[] _nodes)
        {
            row = r;
            column = c;
            nodes = _nodes;
        }
    }
}