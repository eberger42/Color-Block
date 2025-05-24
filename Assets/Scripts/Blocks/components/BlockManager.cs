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

        ITakeBlockCommand _currentEntity;

        [SerializeField]
        private BlockFactory blockFactory; 
        
        private List<Type> primaryColors = new List<Type>
        {
            typeof(RedBlockColor),
            typeof(BlueBlockColor),
            typeof(YellowBlockColor)
        };

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


            if (_currentEntity is IGravity gravityBlock)
            {
                gravityBlock.OnBottomContact -= CreateNewBlock;
            }

            var blockColor = BlockColorFactory.GenerateRandomPrimaryColor();
            var target = blockFactory.CreateBlockGroup(blockColor);

            _currentEntity = target;
            (_currentEntity as IGravity).OnBottomContact += CreateNewBlock;

            Debug.Log("New Block Created");
            OnTargetCreated?.Invoke(target);
        }



    }
}