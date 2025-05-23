using Assets.Scripts.Blocks.interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Blocks.components
{
    public class SquareBarConfigurationStrategy : IBlockGroupConfigurationStrategy
    {
        //Stored As Deltas from GridPosition
        private readonly List<GridPosition> _positions = new List<GridPosition>
        {
            new GridPosition(-1, 0),
            new GridPosition(-1, 1),
            new GridPosition(0, 0),
            new GridPosition(0, 1)
        };

        private readonly GridPosition _pivotPosition = new GridPosition(0, 0);

        List<GridPosition> IBlockGroupConfigurationStrategy.GetPositions()
        {
            return _positions;
        }

        GridPosition IBlockGroupConfigurationStrategy.GetPivotPosition()
        {
            return _pivotPosition;
        }

     
    }
}