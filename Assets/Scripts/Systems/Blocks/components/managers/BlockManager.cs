using Assets.Scripts.Blocks.components.colors;
using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.Data;
using Assets.Scripts.Player.Interfaces;
using Assets.Scripts.Systems.Data;
using Assets.Scripts.Systems.LevelSelect;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Blocks.components
{
    public class BlockManager : MonoBehaviour, ISpawningStrategyListener
    {
        public static BlockManager Instance { get; private set; }
        public IBlockFactory BlockFactory { get => blockFactory; }
        public Dictionary<string, ColorBlockGroupConfiguration> ConfigurationCache { get => _configurationCache; }

        ITakeBlockCommand _currentEntity;

        [SerializeField]
        private BlockFactory blockFactory;
        private ISpawningStrategy spawningStrategy;
        private Dictionary<string, ColorBlockGroupConfiguration> _configurationCache;

        //Events
        public event Action<ITakeBlockCommand> OnTargetCreated;

        int callCount = 0;
        

        private void Awake()
        {


            if (Instance != null)
            {
                Destroy(gameObject);
            }


           Instance = this;


            _configurationCache = new Dictionary<string, ColorBlockGroupConfiguration>();

        }


        private void Start()
        {

            foreach (var config in ColorBlockDataAccessor.Instance.GetAllConfigurations())
            {
                var key = config.id;
                var value = new ColorBlockGroupConfiguration(config);
                _configurationCache[key] = value;
            }
        }
        private void OnDestroy()
        {
            Instance = null;
        }


        public void SetSpawningStrategy(ISpawningStrategy strategy)
        {
            spawningStrategy = strategy;
            spawningStrategy.SpawningSetup(this);
        }

        public void AssignBlockGroupToBlocks(List<IBlock> blocks)
        {
            var blockGroup = blockFactory.AssignBlockGroup();

            blockGroup.Initialize(blocks);

        }

        public void TriggerBlockCreation()
        {

            Debug.Log("CreateNewBlock called " + callCount);
            callCount++;

            if(callCount > 200)
                {
                Debug.LogError("CreateNewBlock called too many times, possible infinite loop");
                return;
            }

            if (_currentEntity is IPlayerControlled gravityBlock)
            {
                gravityBlock.OnPlayerControlCompleted -= BlockManager_OnPlayerControlCompleted;
            }

            var target = spawningStrategy.SpawnBlock(this);

            _currentEntity = target;
            (_currentEntity as IPlayerControlled).OnPlayerControlCompleted += BlockManager_OnPlayerControlCompleted;
            (_currentEntity as IPlayerControlled).SetEnabled(true);

            OnTargetCreated?.Invoke(target);
        }

        ///////////////////////////////////////////////////////////////////
        /// Private Helpers
        ///////////////////////////////////////////////////////////////////

        private void BlockManager_OnPlayerControlCompleted()
        {
            spawningStrategy.HandlePlayerControlCompleted(this);
        }


    }
}