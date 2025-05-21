using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Blocks.interfaces
{
    public interface IEntity
    {
        public void SetWorldPosition(Vector2 position);
        public void SetGridPosition(GridPosition position);
        public void MoveDirection(GridPosition direction);

        public List<GridPosition> GetGridPositions();
    }
}