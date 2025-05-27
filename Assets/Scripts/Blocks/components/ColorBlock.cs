using Assets.Scripts.Blocks.commands;
using Assets.Scripts.Blocks.components.colors;
using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.Grid.components;
using Assets.Scripts.Grid.interfaces;
using Assets.Scripts.Player.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Collections.AllocatorManager;

namespace Assets.Scripts.Blocks.components
{
    public class ColorBlock : TakeBlockCommandMonobehaviour, IBlock, IGravity, ITriggerSpawn, IPlayerControlled
    {
        //Events
        private event Action<GridPosition> _onMoveDirection;
        private event Action<bool> _onEnableGravity;
        private event Action<IBlockColor> _onColorUpdated;
        private event Action _onTriggerSpawn;

        //Fields
        private GridPosition gridPosition;
        private INode node;
        private IBlockGroup parent;

        //Flags
        private bool canTriggerSpawn = false;

        //Properties
        public IBlockColor Color { get; private set; }


        public bool AttemptMerge(ColorBlock colorBlock)
        {

            if (!Color.CanCombine(colorBlock.Color))
                return false;

            var newColor = Color.GetCombineColor(colorBlock.Color);

            (this as IBlock).SetColor(newColor);

            return true;

        }

        public ColorRank GetColorRank()
        {
            return Color.GetColorRank();
        }

        public void SetWorldPosition(Vector2 position)
        {
            this.transform.position = position;
        }

        //Combinte These two, maybe static method to convert grid to world
        public void SetGridPosition(GridPosition position)
        {
            this.gridPosition = position;

            if(gridPosition.y == 0)
            {
                (this as IGravity).SetEnable(false);
            }

        }

        public GridPosition GetGridPosition()
        {
            return gridPosition;
        }

        

        /////////////////////////////////////////////////////////////////
        /// IGravity Interface
        /////////////////////////////////////////////////////////////////

        //Events
        event Action<bool> IGravity.OnEnableGravity
        {
            add
            {
                _onEnableGravity += value;
            }
            remove
            {
                _onEnableGravity -= value;
            }
        }
        
        //Functions
        bool IGravity.CheckIfFloating()
        {
            var southernNode = node.GetNeighbor(GridPosition.Down);
            var isFloating = false;

            if (southernNode ==  null)
                isFloating = true;
            else if (!southernNode.IsOccupied())
                isFloating = true;

            (this as IGravity).SetEnable(isFloating);

            return isFloating;

        }
        void IGravity.SetEnable(bool state)
        {
            if (state == false)
            {
                _onEnableGravity?.Invoke(false);
                RemoveCommandFromFilter(typeof(GravityBlockCommand));
                CheckNeighborsForMerge();

                if (canTriggerSpawn)
                {
                    (this as ITriggerSpawn).SetEnabled(false);
                    _onTriggerSpawn?.Invoke();
                }

            }
            else
            {
                AddCommandToFilter(typeof(GravityBlockCommand));
                _onEnableGravity?.Invoke(true);
                
            }

        }

        /////////////////////////////////////////////////////////////////
        /// IBlock Interface
        /////////////////////////////////////////////////////////////////

        //Events
        event Action<IBlockColor> IBlock.OnColorUpdated
        {
            add
            {
                _onColorUpdated += value;
            }
            remove
            {
                _onColorUpdated -= value;
            }
        }

        //Functions
        void IBlock.SetParent(IBlockGroup parent)
        {
        }
        IBlockGroup IBlock.GetParent()
        {
            return parent;
        }
        void IBlock.SetColor(IBlockColor color)
        {
            this.Color = color;
            _onColorUpdated?.Invoke(color);

        }

        /////////////////////////////////////////////////////////////////
        /// IEntity Interface
        /////////////////////////////////////////////////////////////////
        event Action<GridPosition> IEntity.OnMoveDirection
        {
            add
            {
                _onMoveDirection += value;
            }
            remove
            {
                _onMoveDirection -= value;
            }
        }



        /////////////////////////////////////////////////////////////////
        /// ITakeBlockCommand Interface
        /////////////////////////////////////////////////////////////////

        public override void Place(Grid<BlockNode> colorGrid, GridPosition position)
        {
            
            colorGrid.SetNodeData(position.x, position.y, this);
            (this as IGravity).CheckIfFloating();

        }
        public override void Move(GridPosition direction)
        {

            var canFall = CanTakeCommand(typeof(GravityBlockCommand));
            if (canFall == false)
                return;

            var neigborNode = node.GetNeighbor(direction);
            node.ClearNodeData(this);
            neigborNode.SetNodeData(this);

            (this as IGravity).CheckIfFloating();
        }
        public override void Rotate(GridPosition delta)
        {
            var neigborNode = node.GetNeighbor(delta);
            node.ClearNodeData(this);
            neigborNode.SetNodeData(this);
        }
        public override bool CheckForValidMove(GridPosition direction)
        {
            var isValidMove = !node.IsNeighborOccupied(direction);

            return isValidMove;
        }
        public override bool CheckForValidRotation(GridPosition position)
        {
            var isValidMove = !node.IsNeighborOccupied(position);

            return isValidMove;
        }


        /////////////////////////////////////////////////////////////////
        /// INodeData Interface
        /////////////////////////////////////////////////////////////////
        INode INodeData.GetNodeDataParent()
        {
            return node;
        }

        void INodeData.SetNodeDataParent(INode node)
        {
            this.node = node;
            var gridPosition = node.GetGridPosition();
            var worldPosition = ColorGridManager.Instance.GetWorldPosition(gridPosition);
            SetWorldPosition(worldPosition);
            SetGridPosition(gridPosition);
        }

        void INodeData.HandleNodeEvent(INodeEvent nodeEvent)
        {
            if(nodeEvent is NodeDataRemoved)
            {
                var sender = nodeEvent.GetSender() as INode;

                var vectorTo = Node.VectorTo(node, sender);

                if (vectorTo == GridPosition.Down)
                {
                    (this as IGravity).SetEnable(true);
                }
            }
        }

        ///////////////////////////////////////////////////////////////////
        /// ITriggerSpawn Interface
        ///////////////////////////////////////////////////////////////////

        //Events
        event Action ITriggerSpawn.OnTriggerSpawn
        {
            add
            {
                _onTriggerSpawn += value;
            }
            remove
            {
                _onTriggerSpawn -= value;
            }
        }

        //Functions
        void ITriggerSpawn.SetEnabled(bool state)
        {
            canTriggerSpawn = state;
        }


        ///////////////////////////////////////////////////////////////////
        /// IPlayerControlled Interface
        ///////////////////////////////////////////////////////////////////
        void IPlayerControlled.SetEnabled(bool state)
        {
            if (state)
            {
                PlayerCommands.ForEach(commandType => AddCommandToFilter(commandType));
            }
            else
            {
                PlayerCommands.ForEach(commandType => RemoveCommandFromFilter(commandType));
            }
        }

        ///////////////////////////////////////////////////////////////////
        /// Private Helpers
        ///////////////////////////////////////////////////////////////////

        private void CheckNeighborsForMerge()
        {
            if (Color.GetColorRank() == ColorRank.Secondary)
                return;

            var neighbors = node.GetNeighbors();
            bool markedForDustruction = false;

            if (neighbors == null || neighbors.Count == 0)
                return;

            while (neighbors.Count > 0)
            {
                var neighbor = neighbors[0];
                neighbors.Remove(neighbor);

                if (!neighbor.IsOccupied())
                    continue;

                var block = (neighbor as BlockNode).GetData();

                if (block is ColorBlock colorBlock)
                {
                    var didMerge = colorBlock.AttemptMerge(this);

                    if (didMerge)
                    {
                        markedForDustruction = true;
                    }
                }



            }

            if (markedForDustruction)
            {
                Debug.LogWarning($"ColorBlock {this.Color} marked for destruction.");
                node.ClearNodeData(this);
                Destroy(this.gameObject);
            }

        }

    }
}