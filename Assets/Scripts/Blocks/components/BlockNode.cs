

using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.Blocks.scriptable_objects;
using Assets.Scripts.Grid.components;
using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

namespace Assets.Scripts.Blocks.components
{
    public class BlockNode : Node<IBlock>
    {

        private static GameObject collection = new GameObject("BlockNodeCollection");

        private NodeConfiguration config;
        private GridPosition gridPosition;
        private GameObject gameObject;
        private Grid<BlockNode> gridListener;

        public BlockNode()
        {
            
        }

        public override void Configure(NodeConfiguration config, GridPosition gridPosition)
        {
            this.config = config;
            this.gridPosition = gridPosition;
            this._data = null;

        }

        public ColorRank GetColorRank()
        {
            throw new System.NotImplementedException();
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

        public override GridPosition GetPosition()
        {
            return gridPosition;
        }

        public void SetNode(IBlock block)
        {
        }

        public override void SetNodeData<K>(K nodeData) 
        {
            if (nodeData is IBlock block)
            {

                if(this._data != null)
                    this._data.OnMoveDirection -= MoveDirection;  

                this._data = block;

                this._data.SetWorldPosition(new Vector2(gridPosition.x, gridPosition.y));
                this._data.SetGridPosition(gridPosition);

                this._data.OnMoveDirection += MoveDirection;
                


            }
            else if (nodeData == null)
            {
                if(this._data != null)
                    this._data.OnMoveDirection -= MoveDirection;  

                this._data = null;
            }
            else
            {
                Debug.LogError($"Node data is not of type {typeof(IBlock)}");
            }
        }
        public override void SetGridListener<K>(K gridListener)
        {
            this.gridListener = gridListener as Grid<BlockNode>;
        }


        private void MoveDirection(GridPosition direction)
        {

            var newPosition = new GridPosition(this.gridPosition.x + (int)direction.x, this.gridPosition.y + (int)direction.y);

            var node = gridListener.GetNode(newPosition.x, newPosition.y);

            if(node.IsOccupied())
            {
                return;
            }

            var dataToBeMoved = this._data;
            this.SetNodeData<object>(null);
            node.SetNodeData(dataToBeMoved);

            
        }

        public override bool IsOccupied()
        {
            return _data is IEntity;
        }
    }
}