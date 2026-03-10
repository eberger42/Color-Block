using Assets.Scripts.Blocks.components.colors;
using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.Data;
using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Blocks.components
{

    public class ColorBlockGroupConfiguration : IBlockGroupConfiguration
    {

        private readonly List<ColorBlockConfiguration> _blocks = new List<ColorBlockConfiguration>();
        private readonly GridPosition _pivotPosition;

        public ColorBlockGroupConfiguration(ColorBlockGroupConfigurationData configurationData)
        {

            var blocksData = configurationData.blocks;

            foreach (var blockData in blocksData)
            {
                var gridPosition = new GridPosition(blockData.x, blockData.y);
                var blockColor = BlockColor.ColorTypeToBlockColorCache[blockData.color];

                var blockConfiguration = new ColorBlockConfiguration(blockColor, gridPosition);
                _blocks.Add(blockConfiguration);
            }

        }

        List<ColorBlockConfiguration> IBlockGroupConfiguration.GetPositions()
        {
            return _blocks;
        }

        GridPosition IBlockGroupConfiguration.GetPivotPosition()
        {
            return _pivotPosition;
        }


    }

    public class ColorBlockConfiguration
    {
        private readonly BlockColor _blockColor;
        private readonly GridPosition _position;


        public BlockColor BlockColor => _blockColor;
        public GridPosition Position => _position;


        public ColorBlockConfiguration(BlockColor blockColor, GridPosition position)
        {
            _blockColor = blockColor;
            _position = position;
        }

    }

}