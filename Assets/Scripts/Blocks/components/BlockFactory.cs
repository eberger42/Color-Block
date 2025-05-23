using Assets.Scripts.Blocks.interfaces;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Blocks.components
{
    public class BlockFactory : MonoBehaviour
    {

        [SerializeField]
        private Transform blockPrefab;


        [SerializeField]
        private Transform blockGroupPrefab;

        private readonly List<IBlockGroupConfigurationStrategy> configurationStrategies = new List<IBlockGroupConfigurationStrategy>();
        private void Awake()
        {
            LoadStrategies();
        }

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