using Assets.Scripts.Blocks.interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Blocks.components
{
    public class ColorBlock : MonoBehaviour, IBlock, IColor
    {

        public event Action<GridPosition> OnMoveDirection;

        private GridPosition gridPosition;

        private ColorRank colorRank;
        public ColorRank GetColorRank()
        {
            throw new System.NotImplementedException();
        }

        public void MoveDirection(GridPosition direction)
        {
            this.OnMoveDirection?.Invoke(direction);
        }

        public void SetColorRank(ColorRank rank)
        {
            this.colorRank = rank;
        }

        public void SetWorldPosition(Vector2 position)
        {
            this.transform.position = position;
        }

        public void SetGridPosition(GridPosition position)
        {
            this.gridPosition = position;
        }

        public List<GridPosition> GetGridPositions()
        {
            return new List<GridPosition>() { gridPosition };
        }
    }
}