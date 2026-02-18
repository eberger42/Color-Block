using Assets.Scripts.Blocks.components.colors;
using Assets.Scripts.Blocks.interfaces;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Blocks.components
{
    public class BlockManager : MonoBehaviour
    {
        public static BlockManager Instance { get; private set; }
        public BlockFactory BlockFactory { get => blockFactory; }

        ITakeBlockCommand _currentEntity;

        [SerializeField]
        private BlockFactory blockFactory; 
        
        //Events
        public event Action<ITakeBlockCommand> OnTargetCreated;

        int callCount = 0;
        

        private void Awake()
        {


            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); 

            }
            else
            {
                Destroy(gameObject);
            }
        }


        private void Start()
        {
            CreateNewBlock();
        }


        private void CreateNewBlock()
        {
            callCount++;
            if (callCount > 100)
            {
                Debug.LogWarning("Handler called too many times — breaking loop.");
                return;
            }


            if (_currentEntity is ITriggerSpawn gravityBlock)
            {
                gravityBlock.OnTriggerSpawn -= CreateNewBlock;
            }

            var blockColor = BlockColor.GenerateRandomPrimaryColor();
            var target = blockFactory.CreateBlockGroup(blockColor);
            //var target = blockFactory.CreateBlock(blockColor);

            _currentEntity = target;
            (_currentEntity as ITriggerSpawn).OnTriggerSpawn += CreateNewBlock;
            (_currentEntity as ITriggerSpawn).SetEnabled(true);

            OnTargetCreated?.Invoke(target);
        }

        public void AssignBlockGroupToBlocks(List<IBlock> blocks)
        {
            var blockGroup = blockFactory.AssignBlockGroup();

            blockGroup.Initialize(blocks);

        }

    }
}