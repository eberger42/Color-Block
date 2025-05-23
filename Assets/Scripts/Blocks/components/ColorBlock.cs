using Assets.Scripts.Blocks.commands;
using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.Grid.components;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Collections.AllocatorManager;

namespace Assets.Scripts.Blocks.components
{
    public class ColorBlock : MonoBehaviour, IBlock, IColor, IGravity
    {

        public event Action<GridPosition> OnPositionUpdated;
        public event Action<GridPosition> OnMoveDirection;
        public event Action OnBottomContact;
        public event Action OnMovementBlocked;

        private GridPosition gridPosition;
        private CommandManager commandManager;
        private ColorRank colorRank;
        private INode node;

        private void Awake()
        {
            commandManager = new CommandManager(); 
        }


        public ColorRank GetColorRank()
        {
            throw new System.NotImplementedException();
        }

       
        public void SetColorRank(ColorRank rank)
        {
            this.colorRank = rank;
        }

        public void SetWorldPosition(Vector2 position)
        {
            this.transform.position = position;
        }

        //Combinte These two, maybe static method to convert grid to world
        public void SetGridPosition(GridPosition position)
        {
            this.gridPosition = position;
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
            node.MoveData(direction);
        }
        void ITakeBlockCommand.Place(Grid<BlockNode>  colorGrid, GridPosition position)
        {
            colorGrid.SetNodeData(position.x, position.y, this);
        }

        List<IBlock> ITakeBlockCommand.GetBlocks()
        {
            return new List<IBlock>() { this };
        }

       
    }
}