using Assets.Scripts.Blocks.commands;
using Assets.Scripts.Blocks.components.colors;
using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.Grid.components;
using Assets.Scripts.Grid.interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Collections.AllocatorManager;

namespace Assets.Scripts.Blocks.components
{
    public class ColorBlock : MonoBehaviour, IBlock, IGravity
    {

        public event Action<GridPosition> OnPositionUpdated;
        public event Action<GridPosition> OnMoveDirection;
        public event Action<bool> OnEnableGravity; 
        public event Action OnNeedGravity;
        public event Action OnBottomContact;
        public event Action OnMovementBlocked;

        public event Action<IBlockColor> OnColorUpdated;


        public  IBlockColor Color { get; private set; }

        private GridPosition gridPosition;
        private CommandManager commandManager;
        private INode node;
        private bool canTakeCommands = true;
        private bool canFall = true;

        private IBlockGroup parent;

       

        private void Awake()
        {
            commandManager = new CommandManager(); 
        }

        public bool AttemptMerge(ColorBlock colorBlock)
        {

            if (!Color.CanCombine(colorBlock.Color))
                return false;
            Debug.LogWarning($"Attempting to merge {this.Color} with {colorBlock.Color}");
            Debug.LogWarning($"Parent: {parent} - ColorBlock Parent: {(colorBlock as IBlock).GetParent()}");
            if (parent == (colorBlock as IBlock).GetParent())
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
                OnEnableGravity?.Invoke(false);
            }

            this.OnPositionUpdated?.Invoke(position);
        }

        public GridPosition GetGridPosition()
        {
            return gridPosition;
        }

        

        /////////////////////////////////////////////////////////////////
        /// IGravity Interface
        /////////////////////////////////////////////////////////////////

        bool IGravity.CheckIfFloating()
        {
            var southernNode = node.GetNeighbor(GridPosition.Down);

            if(southernNode ==  null)
                return true;

            if (!southernNode.IsOccupied())
                return true;

            var block = (southernNode as BlockNode).GetData() as IBlock;

            if(parent == block.GetParent())
                return true;

            return false;

        }
        void IGravity.SetEnable(bool state)
        {
            OnEnableGravity?.Invoke(state);
        }

        /////////////////////////////////////////////////////////////////
        /// IBlock Interface
        /////////////////////////////////////////////////////////////////

        void IBlock.SetParent(IBlockGroup parent)
        {
            if(this.parent != null)
                this.parent.OnMergeCheckTriggered -= CheckNeighborsForMerge;

            this.parent = parent;

            if(this.parent != null)
                this.parent.OnMergeCheckTriggered += CheckNeighborsForMerge;
        }

        IBlockGroup IBlock.GetParent()
        {
            return parent;
        }
        void IBlock.SetColor(IBlockColor color)
        {
            this.Color = color;
            OnColorUpdated?.Invoke(color);

        }

        /////////////////////////////////////////////////////////////////
        /// IEntity Interface
        /////////////////////////////////////////////////////////////////
 
        /////////////////////////////////////////////////////////////////
        /// ITakeBlockCommand Interface
        /////////////////////////////////////////////////////////////////
        bool ITakeBlockCommand.CheckForValidMove(GridPosition direction)
        {
            var isValidMove = !node.IsNeighborOccupied(direction);

            return isValidMove;
        }

        void ITakeBlockCommand.Move(GridPosition direction)
        {

            var neigborNode = node.GetNeighbor(direction);
            node.ClearNodeData(this);
            neigborNode.SetNodeData(this);
        }
        void ITakeBlockCommand.Place(Grid<BlockNode> colorGrid, GridPosition position)
        {
            colorGrid.SetNodeData(position.x, position.y, this);
        }

        List<IBlock> ITakeBlockCommand.GetBlocks()
        {
            return new List<IBlock>() { this };
        }

        bool ITakeBlockCommand.CanTakePlayerCommands()
        {
            return canTakeCommands;
        }

        bool ITakeBlockCommand.CanTakeGravityCommands()
        {
            return canFall;
        }

        void ITakeBlockCommand.AddActionCommand(Func<Task> action)
        {
            throw new NotImplementedException();
        }

        bool ITakeBlockCommand.CheckForValidRotation(GridPosition position)
        {
            var isValidMove = !node.IsNeighborOccupied(position);

            return isValidMove;
        }

        void ITakeBlockCommand.Rotate(GridPosition delta)
        {
            var neigborNode = node.GetNeighbor(delta);
            node.ClearNodeData(this);
            neigborNode.SetNodeData(this);
        }


        private void CheckNeighborsForMerge()
        {
            if (Color.GetColorRank() == ColorRank.Secondary)
                return;

            var neighbors = node.GetNeighbors();
            bool markedForDustruction = false;  

            if(neighbors == null || neighbors.Count == 0)
                return;

            while (neighbors.Count > 0)
            {
                var random = new System.Random();
                var neighbor = neighbors[random.Next(neighbors.Count)];
                neighbors.Remove(neighbor);

                if (!neighbor.IsOccupied())
                    continue;


                var block = (neighbor as BlockNode).GetData();


                if (block is ColorBlock colorBlock)
                {

                    var didMerge = colorBlock.AttemptMerge(this);

                    if(didMerge)
                    {
                        markedForDustruction = true;
                    }
                }



            }

            if (markedForDustruction)
            {
                Debug.LogWarning($"ColorBlock {this.Color} marked for destruction.");
                node.ClearNodeData(this);
                parent.ReleaseBlock(this);
                Destroy(this.gameObject);
            }

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
                    OnNeedGravity?.Invoke();
                }
            }
        }

    }
}