using Assets.Scripts.Blocks.components.colors;
using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.Player.Interfaces;
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

        [SerializeReference]
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
            StartCoroutine(SpawnNextFrame());
        }
        private void OnDestroy()
        {
            Instance = null;
        }



        private void CreateNewBlock()
        {
            callCount++;


            if (_currentEntity is IPlayerControlled gravityBlock)
            {
                gravityBlock.OnPlayerControlCompleted -= BlockManager_OnPlayerControlCompleted;
            }

            var blockColor = BlockColor.GenerateRandomPrimaryColor();
            var target = blockFactory.CreateBlockGroup(blockColor);

            _currentEntity = target;
            (_currentEntity as IPlayerControlled).OnPlayerControlCompleted += BlockManager_OnPlayerControlCompleted;
            (_currentEntity as IPlayerControlled).SetEnabled(true);

            OnTargetCreated?.Invoke(target);
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
            CreateNewBlock();
        }

        ///////////////////////////////////////////////////////////////////
        /// Private Helpers
        ///////////////////////////////////////////////////////////////////

        private void BlockManager_OnPlayerControlCompleted()
        {
            spawningStrategy.HandlePlayerControlCompleted(this);
        }

        private IEnumerator SpawnNextFrame()
        {
            yield return null;
            CreateNewBlock();
        }

    }
}