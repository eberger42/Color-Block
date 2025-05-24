using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Blocks.interfaces
{
    public interface IBlock : IEntity, ITakeBlockCommand
    {
        public GridPosition GetGridPosition();

        public void SetColor(IBlockColor color);
    }
}