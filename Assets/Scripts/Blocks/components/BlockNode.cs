

using Assets.Scripts.Blocks.commands;
using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.Blocks.scriptable_objects;
using Assets.Scripts.Grid.components;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

namespace Assets.Scripts.Blocks.components
{
    public class BlockNode : Node<IEntity>
    {

        private static GameObject collection = new GameObject("BlockNodeCollection");

        private NodeConfiguration config;
        private GridPosition gridPosition;
        private GameObject gameObject;
        private Grid<BlockNode> gridListener;

        public GridPosition GridPosition { get => gridPosition; }

        public BlockNode()
        {
            
        }


        public IEntity GetData()
        {
            return _data;
        }

        public override void Configure(NodeConfiguration config, GridPosition gridPosition)
        {
            this.config = config;
            this.gridPosition = gridPosition;
            this._data = null;

        }


        public override void GenerateNode()
        {

            var size = config.Size;
            var origin = config.Origin;
            var color = config.Color;
            var texture = config.Texture;

            gameObject = new GameObject($"BlockNode[{gridPosition.x},{gridPosition.y}]");
            gameObject.transform.parent = collection.transform;
            gameObject.transform.position = new Vector2(gridPosition.x * size + origin.x, gridPosition.y * size + origin.y);
            var spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, size*64, size*64), new Vector2(0.5f, 0.5f));
            spriteRenderer.color = color;
            
        }

        public override void SetPosition(GridPosition position)
        {

        }

        public override GridPosition GetGridPosition()
        {
            return gridPosition;
        }

        public override void SetNodeData<K>(K nodeData) 
        {
            if (nodeData is IBlock block)
            {
                this._data = block;

                if((this._data as MonoBehaviour))
                    this._data.SetGridNode(this);

            }
            else
            {
                Debug.LogError($"Node data is not of type {typeof(IBlock)}");
            }
        }

        public override void ClearNodeData<K>(K nodeData)
        {
            if (nodeData is IBlock block)
            {
                if (this._data == block)
                {
                    this._data = null;
                }
            }
        }

        public override void SetGridListener<K>(K gridListener)
        {
            this.gridListener = gridListener as Grid<BlockNode>;
        }


        public override INode GetNeighbor(GridPosition direction)
        {
            var newPosition = new GridPosition(this.gridPosition.x + (int)direction.x, this.gridPosition.y + (int)direction.y);

            var node = gridListener.GetNode(newPosition.x, newPosition.y);

            return node;

        }

        public override List<INode> GetNeighbors()
        {
            var nodes = new List<INode>();  

            foreach(var direction in GridPosition.Directions)
            {
                var node = GetNeighbor(direction);

                if(node == null)
                    continue;

                nodes.Add(node);
            }
            return nodes;

        }

        public override INode GetRotationNode(GridPosition targetPosition)
        {
            var newPosition = new GridPosition(this.gridPosition.x + (int)targetPosition.x, this.gridPosition.y + (int)targetPosition.y);

            var node = gridListener.GetNode(newPosition.x, newPosition.y);

            return node;
        }

        public override bool IsOccupied()
        {
            return _data is IEntity;
        }

        public override bool IsNeighborOccupied(GridPosition direction)
        {
            var neighborPosition = new GridPosition(this.gridPosition.x + (int)direction.x, this.gridPosition.y + (int)direction.y);   
            
            var isInBounds = gridListener.IsInBounds(neighborPosition.x, neighborPosition.y);

            if (!isInBounds)
                return true;

            var neighborNode = gridListener.GetNode(neighborPosition.x, neighborPosition.y);
            return neighborNode.IsOccupied();
        }
    }
}