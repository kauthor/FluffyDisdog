using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FluffyDisdog
{
    public enum NodeState
    {
        Raw=0,
        Digged=1,
    }
    public class TerrainNode:MonoBehaviour
    {
        [SerializeField] private bool isObstacle=false;
        private Tuple<int, int> coord;
        public Tuple<int, int> Coord => coord;
        private Action<Tuple<int, int>> onClicked;
        [SerializeField]private SpriteRenderer _renderer;

        private NodeState currentState;

        

        public void InitNode(int row, int col, Action<Tuple<int, int>> cb)
        {
            coord = new Tuple<int, int>(row, col);
            onClicked = cb;
            currentState = NodeState.Raw;
        }

        private void OnMouseDown()
        {
            onClicked?.Invoke(coord);
        }

        public void ChangeState()
        {
            if (isObstacle)
            {
                return;
            }

            if (currentState == NodeState.Digged)
                return;

            currentState = NodeState.Digged;
            _renderer.sprite = null;

            //여기에서 게임 매니저에 점수 호출
        }

        public bool ValidNode() => currentState == NodeState.Raw;
    }
}