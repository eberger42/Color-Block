using Assets.Scripts.Blocks.components;
using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.Blocks.scriptable_objects;
using Assets.Scripts.General;
using Assets.Scripts.General.interfaces;
using Assets.Scripts.Grid.interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Grid.components
{
    public abstract class Node : INode, IDispose
    {

        public static GridPosition VectorTo(INode startNode, INode endNode)
        {
            var startPos = startNode.GetGridPosition();
            var endPos = endNode.GetGridPosition();
            return new GridPosition(endPos.x - startPos.x, endPos.y - startPos.y);
        }

        //Events
        public event Action<INodeEvent> OnNodeEvent;

        private static GameObject collection = new GameObject($"{typeof(INodeData)}NodeCollection");

        protected IGrid<INode> gridListener;
        protected INodeData _data;
        protected NodeConfiguration config;
        protected GridPosition gridPosition;
        protected GameObject gameObject;

        public GridPosition GridPosition { get => gridPosition; }

        protected Dictionary<GridPosition, INode> neighbors = new Dictionary<GridPosition, INode>();


        public virtual INodeData GetData()
        {
            return _data;
        }

        public virtual void Configure(NodeConfiguration config, GridPosition gridPosition)
        {
            this.config = config;
            this.gridPosition = gridPosition;
            this._data = default;

        }

        public virtual void GenerateNode()
        {

            var size = config.Size;
            var origin = config.Origin;
            var color = config.Color;
            var texture = config.Texture;

            gameObject = new GameObject($"BlockNode[{gridPosition.x},{gridPosition.y}]");
            gameObject.transform.parent = collection.transform;
            gameObject.transform.position = new Vector2(gridPosition.x * size + origin.x, gridPosition.y * size + origin.y);
            var spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, size * 64, size * 64), new Vector2(0.5f, 0.5f));
            spriteRenderer.color = color;

            var neigbors = this.GetNeighbors();

            foreach ( var neighbor in neigbors)
            {
                   if (neighbor == default) continue;

                neighbor.OnNodeEvent += OnNodeDataEvent;
            }


        }

        public virtual GridPosition GetGridPosition()
        {
            return gridPosition;
        }

        public virtual void SetNodeData(INodeData nodeData)
        {
            this._data = nodeData;
            OnNodeEvent?.Invoke(new NodeDataSet(this));
        }

        public virtual void ClearNodeData(INodeData nodeData)
        {
            if (this._data == nodeData)
            {
                var removedData = this._data;
                this._data = default;
                OnNodeEvent?.Invoke(new NodeDataRemoved(this, removedData));
            }
        }
        
        public virtual void SetGridListener(IGrid<INode> gridListener)
        {
            this.gridListener = gridListener;
        }


        public virtual INode GetNeighbor(GridPosition direction)
        {
            var newPosition = new GridPosition(this.gridPosition.x + (int)direction.x, this.gridPosition.y + (int)direction.y);

            var node = gridListener.GetNode(newPosition.x, newPosition.y);

            return node;

        }


        public virtual List<INode> GetNeighbors()
        {
            var nodes = new List<INode>();

            foreach (var direction in GridPosition.Directions)
            {
                var node = GetNeighbor(direction);

                if (node == null)
                    continue;

                nodes.Add(node);
            }
            return nodes;

        }


        public virtual INode GetRotationNode(GridPosition targetPosition)
        {
            var newPosition = new GridPosition(this.gridPosition.x + (int)targetPosition.x, this.gridPosition.y + (int)targetPosition.y);

            var node = gridListener.GetNode(newPosition.x, newPosition.y);

            return node;
        }

        public virtual bool IsOccupied()
        {
            return _data is INodeData;
        }

        public virtual bool IsNeighborOccupied(GridPosition direction)
        {
            var neighborPosition = new GridPosition(this.gridPosition.x + (int)direction.x, this.gridPosition.y + (int)direction.y);

            var isInBounds = gridListener.IsInBounds(neighborPosition.x, neighborPosition.y);

            if (!isInBounds)
                return true;

            var neighborNode = gridListener.GetNode(neighborPosition.x, neighborPosition.y);
            return neighborNode.IsOccupied();
        }

        public abstract void OnNodeDataEvent(INodeEvent nodeEvent);

        protected void TriggerNodeEvent(INodeEvent nodeEvent)
        {
            OnNodeEvent?.Invoke(nodeEvent);
        }

        ///////////////////////////////////////////////////////////
        /// ITick Interface
        ///////////////////////////////////////////////////////////
        public abstract void Tick();

        void IDispose.Dispose()
        {
            foreach (var neighbor in neighbors.Values)
            {
                if (neighbor == null) continue;
                    neighbor.OnNodeEvent -= OnNodeDataEvent;
            }

        }

    }
}