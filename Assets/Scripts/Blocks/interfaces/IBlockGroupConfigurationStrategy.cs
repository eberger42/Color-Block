using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Blocks.interfaces
{
    public interface IBlockGroupConfigurationStrategy
    {
        List<GridPosition> GetPositions();

        GridPosition GetPivotPosition();
        
    }
}