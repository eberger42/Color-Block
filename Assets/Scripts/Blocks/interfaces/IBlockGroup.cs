using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Blocks.interfaces
{
    public interface IBlockGroup : IEntity
    {

        public void GetPivotBlock(IBlock block);

        public void RotateBlockGroup(Vector2Int direction);

        public void DestroyBlockGroup();

        public void ReleaseBlock(IBlock block);

    }
}