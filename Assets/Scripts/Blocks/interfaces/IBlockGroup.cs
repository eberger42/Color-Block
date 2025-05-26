using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Blocks.interfaces
{
    public interface IBlockGroup : ITakeBlockCommand
    {

        public event Action OnMergeCheckTriggered;
        public void AddBlock(IBlock block, GridPosition delta);
        public void ReleaseBlock(IBlock block);
        public void Disband();
        public List<GridPosition> GetGridPositions();
        public void SetColor(IBlockColor color);

    }
}