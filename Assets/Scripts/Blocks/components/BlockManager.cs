using Assets.Scripts.Blocks.interfaces;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Blocks.components
{
    public class BlockManager : MonoBehaviour
    {
        public static BlockManager Instance { get; private set; }

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


            if (_currentEntity is IGravity gravityBlock)
            {
                gravityBlock.OnBottomContact -= CreateNewBlock;
            }

            var target = blockFactory.CreateBlockGroup(Color.red);

            _currentEntity = target;
            (_currentEntity as IGravity).OnBottomContact += CreateNewBlock;

            Debug.Log("New Block Created");
            OnTargetCreated?.Invoke(target);
        }



    }
}