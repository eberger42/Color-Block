using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Blocks.interfaces
{
    public interface IBlockGroup
    {
        public event Action<GridPosition> OnPositionUpdated;


        public event Action OnMergeCheckTriggered;
        public void AddBlock(IBlock block, GridPosition delta);
        public void ReleaseBlock(IBlock block);
        public void Disband();
        public void SetColor(IBlockColor color);

    }
}