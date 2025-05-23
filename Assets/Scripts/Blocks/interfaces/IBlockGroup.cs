using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Blocks.interfaces
{
    public interface IBlockGroup : IEntity, ITakeBlockCommand
    {

        public void AddBlock(IBlock block);

        public void GetPivotBlock(IBlock block);

        public void RotateBlockGroup(Vector2Int direction);

        public void DestroyBlockGroup();

        public void ReleaseBlock(IBlock block);
        public List<GridPosition> GetGridPositions();

    }
}