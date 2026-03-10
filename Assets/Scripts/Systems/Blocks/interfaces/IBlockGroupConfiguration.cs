using Assets.Scripts.Blocks.components;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Blocks.interfaces
{
    public interface IBlockGroupConfiguration
    {
        List<ColorBlockConfiguration> GetPositions();

        GridPosition GetPivotPosition();
        
    }
}