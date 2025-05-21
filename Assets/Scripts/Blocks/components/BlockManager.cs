using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Blocks.components
{
    public class BlockManager : MonoBehaviour
    {
        public static BlockManager Instance { get; private set; }


        [SerializeField]
        private BlockFactory blockFactory;  



        //Events
        public event Action<ColorBlock> OnBlockCreated;

        

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
            var block = blockFactory.CreateBlock(Color.red);
            OnBlockCreated?.Invoke(block);
        }
    }
}