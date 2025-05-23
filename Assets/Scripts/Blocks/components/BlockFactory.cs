using Assets.Scripts.Blocks.interfaces;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Blocks.components
{
    public class BlockFactory : MonoBehaviour
    {

        [SerializeField]
        private Transform blockPrefab;


        [SerializeField]
        private Transform blockGroupPrefab;

        private readonly List<IBlockGroupConfigurationStrategy> configurationStrategies = new List<IBlockGroupConfigurationStrategy>
        {
            new LongBarConfigurationStrategy()
        };

        public ITakeBlockCommand CreateBlock(Color color)
        {
            ColorBlock block = Instantiate(blockPrefab, new Vector2(0,0), Quaternion.identity).GetComponent<ColorBlock>();
            block.SetColorRank(ColorRank.Primary);
            return block;
        }

        public ITakeBlockCommand CreateBlockGroup(Color color)
        {
            ColorBlockGroup blockGroup = Instantiate(blockGroupPrefab, new Vector2(0, 0), Quaternion.identity).GetComponent<ColorBlockGroup>();
 
            var configurationStrategy = GetRandomConfigurationStrategy();
            blockGroup.Initialize(configurationStrategy, this);
            return blockGroup;
        }


        private IBlockGroupConfigurationStrategy GetRandomConfigurationStrategy()
        {
            var randomIndex = Random.Range(0, configurationStrategies.Count);
            return configurationStrategies[randomIndex];
        }
    }
}