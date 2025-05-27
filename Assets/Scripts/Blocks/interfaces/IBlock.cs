using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Blocks.interfaces
{
    public interface IBlock : IEntity
    {

        public event Action<IBlockColor> OnColorUpdated;

        public GridPosition GetGridPosition();
        public void SetColor(IBlockColor color);
        public void SetParent(IBlockGroup parent);
        public IBlockGroup GetParent();
    }
}