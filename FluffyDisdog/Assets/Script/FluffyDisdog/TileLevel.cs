﻿using System;
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
            nodes = GetComponentsInChildren<TerrainNode>();
        }
    }
}