
using Assets.Scripts.Data;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Blocks.components.managers
{
    public class PuzzleBlockQueue
    {
        private Queue<ColorBlockGroupConfiguration> _queue = new Queue<ColorBlockGroupConfiguration>();
     

        public PuzzleBlockQueue(List<ColorBlockGroupConfiguration> blockGroupConfigs)
        {
            foreach(var config in blockGroupConfigs)
            {
                _queue.Enqueue(config);
            }
        }

        public ColorBlockGroupConfiguration GetNextBlockGroup()
        {
            if (_queue.Count > 0)
            {
                return _queue.Dequeue();
            }
            else
            {
                Debug.LogWarning("PuzzleBlockQueue is empty. No more block groups to spawn.");
                return null;
            }
        }

        public void ClearQueue()
        {
            _queue.Clear();
        }
    }
}
