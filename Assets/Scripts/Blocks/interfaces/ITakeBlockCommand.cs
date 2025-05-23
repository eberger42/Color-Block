using Assets.Scripts.Blocks.components;
using Assets.Scripts.Grid.components;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;
using UnityEngine;

namespace Assets.Scripts.Blocks.interfaces
{
    public interface ITakeBlockCommand
    {

        public event Action<GridPosition> OnPositionUpdated;

        public bool CheckForValidMove(GridPosition direction);
        public void Move(GridPosition direction);
        public void Place(Grid<BlockNode>  colorGrid, GridPosition position);

        public List<IBlock> GetBlocks();
    }
}