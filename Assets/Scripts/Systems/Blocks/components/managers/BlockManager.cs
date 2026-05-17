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
    public class BlockManager : MonoBehaviour, ISpawningStrategyListener, ITakeFire1Input
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

        //////////////////////////////////////////////////////////////////
        /// Unity Lifecycle
        //////////////////////////////////////////////////////////////////
        #region Unity Lifecycle 
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

        #endregion


        //////////////////////////////////////////////////////////////////
        /// Public Methods
        ///////////////////////////////////////////////////////////////////

        #region Public Methods
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

            try
            {

                callCount++;

                if (callCount > 200)
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
            catch (Exception ex)
            {
                Debug.LogError($"Error in BlockManager.TriggerBlockCreation: {ex.Message}");
            }

         }
        #endregion

        //////////////////////////////////////////////////////////////////
        /// ITakeFire1Input Implementation
        //////////////////////////////////////////////////////////////////

        #region ITakeFire1Input Implementation
        void ITakeFire1Input.HandleFire1Logic(object listener)
        {
            spawningStrategy.HandleFire1Logic(this);
        }
        #endregion

        ///////////////////////////////////////////////////////////////////
        /// Private Helpers
        ///////////////////////////////////////////////////////////////////

        private void BlockManager_OnPlayerControlCompleted()
        {
            spawningStrategy.HandlePlayerControlCompleted(this);
        }

    }
}