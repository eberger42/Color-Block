using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.Grid.components;
using Assets.Scripts.Grid.interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Blocks.components.managers
{
    public class ColorChainManager : MonoBehaviour
    {
        private List<List<IBlock>> colorChainList = new List<List<IBlock>>();
        private Dictionary<IBlock, List<IBlock>> blockChainListMapping = new Dictionary<IBlock, List<IBlock>>();

        [SerializeField]
        private Score _scoreTarget;

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

        private void CheckForColorChain(INodeEvent nodeEvent)
        {


            var node = nodeEvent.GetSender();
            var block = node.GetData() as IBlock;
            var neighbors = node.GetNeighbors();

            List<IBlock> newColorChain = new List<IBlock>
            {
                block
            };

            var neighborChain = SearchNode(node, new List<INode>());

            newColorChain.AddRange(neighborChain);

            if (newColorChain.Count > 5)
            {
                foreach (var chainBlock in newColorChain)
                {
                    chainBlock.Remove();
                }
                _scoreTarget.AddToScore(newColorChain.Count);
            }

        }
        private List<IBlock> SearchNode(INode node, List<INode> searchedNodes)
        {
            var neighbors = node.GetNeighbors();
            var block = node.GetData() as IBlock;
            List<IBlock> colorChain = new List<IBlock>();
            searchedNodes.Add(node);

            if(block == null)
                return colorChain;

            foreach (var neighbor in neighbors)
            {
                if (searchedNodes.Contains(neighbor))
                    continue;
                if (!neighbor.IsOccupied())
                    continue;

                var neighborBlock = neighbor.GetData() as IBlock;

                var doColorsMatch = block.DoColorsMatch(neighborBlock);

                if (doColorsMatch)
                {
                    colorChain.Add(neighborBlock);
                    var neighborChain = SearchNode(neighbor, searchedNodes);
                    colorChain.AddRange(neighborChain);
                }
            }
            return colorChain;
        }

        private void HandleNodeEvent(INodeEvent nodeEvent)
        {
            if (nodeEvent is NodeDataColorChanged colorChangedEvent)
            {
                StartCoroutine(DeferredColorChainCheck(colorChangedEvent));

                //AddBlockToColorChain(colorChangedEvent);
            }
            else if (nodeEvent is NodeDataRemoved removedEvent)
            {
                var node = removedEvent.GetSender();
                var block = removedEvent.RemovedData as IBlock;

                if (blockChainListMapping.TryGetValue(block, out var colorChain))
                {
                    Debug.Log($"Removing block from chain: {block}");
                    colorChainList.Remove(colorChain);
                    blockChainListMapping.Remove(block);
                }
            }
            else if (nodeEvent is NodeDataLanded landedEvent)
            {
                CheckForColorChain(nodeEvent);
            }
        }
        private IEnumerator DeferredColorChainCheck(NodeDataColorChanged evt)
        {
            yield return null; // wait 1 frame
            CheckForColorChain(evt);
        }



    }

}