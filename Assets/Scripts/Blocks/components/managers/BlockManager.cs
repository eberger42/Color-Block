using Assets.Scripts.Blocks.components.colors;
using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.Player.Interfaces;
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

        ITakeBlockCommand _currentEntity;

        [SerializeField]
        private BlockFactory blockFactory;

        private ISpawningStrategy spawningStrategy;

        //Events
        public event Action<ITakeBlockCommand> OnTargetCreated;

        int callCount = 0;
        

        private void Awake()
        {


            if (Instance == null)
            {
                Instance = this;

            }
            else
            {
                Destroy(gameObject);
            }
       
           

        }


        private void Start()
        {
            if (SceneController.instance.GetCurrentScene() == Scenes.FreePlay)
            {
                spawningStrategy = new FreePlaySpawningStrategy();
            }
            else
            {
                spawningStrategy = new PuzzleSpawningStrategy(LevelSelectManager.Instance);
            }

            spawningStrategy.SpawningSetup(this);
        }
        private void OnDestroy()
        {
            Instance = null;
        }


        public void AssignBlockGroupToBlocks(List<IBlock> blocks)
        {
            var blockGroup = blockFactory.AssignBlockGroup();

            blockGroup.Initialize(blocks);

        }


        ///////////////////////////////////////////////////////////////////
        /// ISpawningStrategyListener Implementation
        ///////////////////////////////////////////////////////////////////
        void ISpawningStrategyListener.CreateNewBlock()
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

            var target = spawningStrategy.SpawnBlock(this, "");

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