using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Blocks.interfaces
{
    public interface IBlock : IEntity
    {
        public event Action<GridPosition> OnMoveDirection;
    }
}