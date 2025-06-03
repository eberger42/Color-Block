using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.Grid.components;
using Assets.Scripts.Grid.interfaces;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Blocks.components.managers
{
    public class ColorChainManager : MonoBehaviour
    {
        private List<List<IBlock>> colorChainList = new List<List<IBlock>>();
        private Dictionary<IBlock, List<IBlock>> blockChainListMapping = new Dictionary<IBlock, List<IBlock>>();



        public void Awake()
        {
        }

        private void Start()
        {
            ColorGridManager.Instance.OnNodeEvent += HandleNodeEvent;

        }

        private void OnDestroy()
        {
            ColorGridManager.Instance.OnNodeEvent -= HandleNodeEvent;
        }

        private void AddBlockToColorChain(INodeEvent nodeEvent)
        {
            var node = nodeEvent.GetSender();
            var block = node.GetData() as IBlock;
            var neighbors = node.GetNeighbors();

            List<IBlock> newColorChain = new List<IBlock>();
            colorChainList.Add(newColorChain);
            blockChainListMapping[block] = newColorChain;

            newColorChain.Add(block);

            foreach (var neighbor in neighbors)
            {

                if (!neighbor.IsOccupied())
                    continue;

                var neighborBlock = neighbor.GetData() as IBlock;

                var doColorsMatch = block.DoColorsMatch(neighborBlock);

                if(doColorsMatch && blockChainListMapping.ContainsKey(neighborBlock))
                {
                    var neighborsColorChain = blockChainListMapping[neighborBlock];
                    newColorChain.AddRange(neighborsColorChain);

                    colorChainList.Remove(neighborsColorChain);
                    blockChainListMapping[block] = newColorChain;
                }
            };
            if(newColorChain.Count > 5)
            {
                foreach (var chainBlock in newColorChain)
                {
                    blockChainListMapping.Remove(chainBlock);
                    (chainBlock as IEntity).Destroy();
                }
                colorChainList.Remove(newColorChain);
            }

        }

        private void HandleNodeEvent(INodeEvent nodeEvent)
        {
            if (nodeEvent is NodeDataColorChanged colorChangedEvent)
            {
                AddBlockToColorChain(colorChangedEvent);
            }
        }
        

    }

}