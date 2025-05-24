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
    public class BlockFactory : MonoBehaviour
    {
        private static int _blockCount = 0;
        [SerializeField]
        private Transform blockPrefab;


        [SerializeField]
        private Transform blockGroupPrefab;

        private readonly List<IBlockGroupConfigurationStrategy> configurationStrategies = new List<IBlockGroupConfigurationStrategy>();
        private void Awake()
        {
            LoadStrategies();
        }

        public ITakeBlockCommand CreateBlock(IBlockColor blockColor)
        {
            ColorBlock block = Instantiate(blockPrefab, new Vector2(0,0), Quaternion.identity).GetComponent<ColorBlock>();
            block.transform.name = $"ColorBlock: {_blockCount}";
            (block as IBlock).SetColor(blockColor);

            _blockCount++;
            return block;
        }

        public ITakeBlockCommand CreateBlockGroup(IBlockColor blockColor)
        {
            ColorBlockGroup blockGroup = Instantiate(blockGroupPrefab, new Vector2(0, 0), Quaternion.identity).GetComponent<ColorBlockGroup>();
 
            var configurationStrategy = GetRandomConfigurationStrategy();
            blockGroup.Initialize(configurationStrategy, this, blockColor);
            return blockGroup;
        }


        private IBlockGroupConfigurationStrategy GetRandomConfigurationStrategy()
        {
            var randomIndex = Random.Range(0, configurationStrategies.Count);
            return configurationStrategies[randomIndex];
        }

        private void LoadStrategies()
        {
            var interfaceType = typeof(IBlockGroupConfigurationStrategy);

            // Search all loaded assemblies (typically just Assembly-CSharp)
            var strategyTypes = System.AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => !type.IsAbstract && !type.IsInterface && interfaceType.IsAssignableFrom(type));

            foreach (var type in strategyTypes)
            {
                try
                {
                    var instance = System.Activator.CreateInstance(type) as IBlockGroupConfigurationStrategy;
                    if (instance != null)
                        configurationStrategies.Add(instance);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Could not instantiate {type.Name}: {e.Message}");
                }
            }

            Debug.Log($"Loaded {configurationStrategies.Count} strategy(ies).");
        }
    }
}