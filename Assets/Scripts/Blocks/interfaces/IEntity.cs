using Assets.Scripts.Grid.interfaces;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Blocks.interfaces
{
    public interface IEntity : INodeData
    {
        public event Action<GridPosition> OnMoveDirection;
        public event Action<IEntity> OnEntityDestroyed;

        void Destroy();
    }
}