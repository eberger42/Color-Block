using Assets.Scripts.Blocks.commands;
using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.Grid.components;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Blocks.components
{
    public class ColorBlockGroup : MonoBehaviour, IBlockGroup, IColor, IGravity
    {
        public event Action OnBottomContact;
        public event Action<GridPosition> OnMoveDirection;
        public event Action<GridPosition> OnPositionUpdated;


        private Dictionary<IBlock,GridPosition> _positionsDeltaMap = new Dictionary<IBlock, GridPosition>();
        private IBlock pivotBlock;
        private List<IBlock> blocks = new List<IBlock>();
        private CommandManager commandManager;

        private void Awake()
        {
            commandManager = new CommandManager();
        }

        public void Initialize(IBlockGroupConfigurationStrategy configurationStrategy, BlockFactory factory)
        {
            var _positions = configurationStrategy.GetPositions();
            var pivotPosition = configurationStrategy.GetPivotPosition();

            foreach (var position in _positions)
            {
                var block = factory.CreateBlock(Color.red) as IBlock;
                blocks.Add(block);

                _positionsDeltaMap.Add(block, position);

                if (position == pivotPosition)
                {
                    pivotBlock = block;
                }
            }
        }

        void IBlockGroup.DestroyBlockGroup()
        {
            throw new NotImplementedException();
        }
        void IBlockGroup.ReleaseBlock(IBlock block)
        {
            throw new NotImplementedException();
        }

        void IBlockGroup.RotateBlockGroup(Vector2Int direction)
        {
            throw new NotImplementedException();
        }
        void IBlockGroup.GetPivotBlock(IBlock block)
        {
            throw new NotImplementedException();
        }

        void IBlockGroup.AddBlock(IBlock block)
        {
            this.blocks.Add(block);
        }

        void IColor.SetColorRank(ColorRank rank)
        {
            throw new NotImplementedException();
        }

        ColorRank IColor.GetColorRank()
        {
            throw new NotImplementedException();
        }

        List<GridPosition> IBlockGroup.GetGridPositions()
        {
            List<GridPosition> gridPositions = new List<GridPosition>();
            foreach (var block in blocks)
            {
                var gridPosition = block.GetGridPosition();
                gridPositions.Add(gridPosition);
                
            }

            return gridPositions;
        }

        ///////////////////////////////////////////////////////////////////
        /// IBlockGroup Interface
        ///////////////////////////////////////////////////////////////////
        void ITakeBlockCommand.Place(Grid<BlockNode>  colorGrid, GridPosition position)
        {
            foreach (var block in blocks)
            {
                var delta = _positionsDeltaMap[block];
                var newPosition = position + delta;
                block.Place(colorGrid, newPosition);
            }

            this.OnPositionUpdated?.Invoke(position);
        }

        void ITakeBlockCommand.Move(GridPosition direction)
        {
            foreach (var block in blocks)
            {
                block.Move(direction);
            }
        }

        bool ITakeBlockCommand.CheckForValidMove(GridPosition direction)
        {
            foreach (var block in blocks)
            {

                var targetPosition = _positionsDeltaMap[block] + direction;

                if(_positionsDeltaMap.ContainsValue(targetPosition)) //Check if the target position is occupied by another block in the group
                    continue;


                var isValid = block.CheckForValidMove(direction);

                if (!isValid)
                {
                    Debug.Log($"Invalid Move for Block: {block}");
                    return false;
                }
            }
            return true;
        }
        List<IBlock> ITakeBlockCommand.GetBlocks()
        {
            return blocks;
        }

        void IGravity.TriggerBottomReahed()
        {
            
        }

        void IEntity.SetGridNode(INode node)
        {
            //Noop
        }

    }
}