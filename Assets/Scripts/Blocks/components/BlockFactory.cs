using Assets.Scripts.Blocks.components.colors;
using Assets.Scripts.Blocks.interfaces;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

namespace Assets.Scripts.Blocks.components
{
   
    public class BlockFactory : MonoBehaviour, IBlockFactory
    {
        private static int _blockCount = 0;
        private static int _groupCount = 0;
        [SerializeField]
        private Transform blockPrefab;


        [SerializeField]
        private Transform blockGroupPrefab;

        private readonly List<IBlockGroupConfiguration> configurationStrategies = new List<IBlockGroupConfiguration>();
        private void Awake()
        {

        }

        public ITakeBlockCommand CreateBlock(IBlockColor blockColor)
        {
            ColorBlock block = Instantiate(blockPrefab, new Vector2(0, 0), Quaternion.identity).GetComponent<ColorBlock>();
            block.transform.name = $"ColorBlock: {_blockCount}";
            (block as IBlock).SetColor(blockColor);

            _blockCount++;
            return block;
        }

        public ITakeBlockCommand CreateBlockGroup()
        {
            _groupCount++;

            var gameObject = Instantiate(blockGroupPrefab, new Vector2(0, 0), Quaternion.identity).GetComponent<ColorBlockGroupController>();
            var blockGroup = gameObject.GetComponent<ColorBlockGroupController>();

            blockGroup.transform.name = $"ColorBlockGroup: {_groupCount}";

            return blockGroup;
        }

        public ColorBlockGroupController AssignBlockGroup()
        {
            _groupCount++;

            var gameObject = Instantiate(blockGroupPrefab, new Vector2(0, 0), Quaternion.identity).GetComponent<ColorBlockGroupController>();
            var blockGroup = gameObject.GetComponent<ColorBlockGroupController>();

            blockGroup.transform.name = $"ColorBlockGroup: {_groupCount}";
            return blockGroup;
        }

    }
}