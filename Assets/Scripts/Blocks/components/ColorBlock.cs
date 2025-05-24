using Assets.Scripts.Blocks.commands;
using Assets.Scripts.Blocks.components.colors;
using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.Grid.components;
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
        public event Action OnBottomContact;
        public event Action OnMovementBlocked;

        public event Action<IBlockColor> OnColorUpdated;


        private IBlockColor color;

        private GridPosition gridPosition;
        private CommandManager commandManager;
        private INode node;
        private bool canTakeCommands = true;
        private bool canFall = true;

        private void Awake()
        {
            commandManager = new CommandManager(); 
        }


        public ColorRank GetColorRank()
        {
            return color.GetColorRank();
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
                OnBottomContact?.Invoke();

            this.OnPositionUpdated?.Invoke(position);
        }

        public GridPosition GetGridPosition()
        {
            return gridPosition;
        }

        public void TriggerBottomReahed()
        {
            OnBottomContact?.Invoke();
        }

        /////////////////////////////////////////////////////////////////
        /// IBlock Interface
        /////////////////////////////////////////////////////////////////

        void IBlock.SetColor(IBlockColor color)
        {
            this.color = color;
            OnColorUpdated?.Invoke(color);

        }

        /////////////////////////////////////////////////////////////////
        /// IEntity Interface
        /////////////////////////////////////////////////////////////////
        void IEntity.SetGridNode(INode node)
        {
            this.node = node;
            var gridPosition = node.GetGridPosition();
            var worldPosition = ColorGridManager.Instance.GetWorldPosition(gridPosition);
            SetWorldPosition(worldPosition);
            SetGridPosition(gridPosition);
        }

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
            var neigborNode = node.GetRotationNode(delta);
            node.ClearNodeData(this);
            neigborNode.SetNodeData(this);
        }

    }
}