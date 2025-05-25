using Assets.Scripts.Blocks.commands;
using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.Grid.components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.Blocks.components
{
    /// <summary>
    /// The task Queue is used to execute the commands in order, one at a time. Preventing blocks phasing through each other
    /// </summary>
    public class ColorBlockGroup : MonoBehaviour, IBlockGroup, IGravity
    {
        public event Action OnBottomContact;
        public event Action OnNeedGravity;
        public event Action OnMergeCheckTriggered;
        public event Action<GridPosition> OnMoveDirection;
        public event Action<GridPosition> OnPositionUpdated;


        //Block Positioning Helpers
        private Dictionary<IBlock,GridPosition> _positionsDeltaMap = new Dictionary<IBlock, GridPosition>();
        private Dictionary<IBlock, GridPosition> _positionsRotationUpdateDeltaMap;

        
        //Grouped elements
        private List<IBlock> blocks = new List<IBlock>();

        //Command/Action Queue
        private Queue<Func<Task>> _actionQueue = new Queue<Func<Task>>();

        //Flags
        private bool ignoreOtherBottomReachedEvents = false;
        private bool canTakeCommands = true;
        private bool canFall = true;
        private bool executingAction = false;


        private void Awake()
        {
        }

        public void Initialize(IBlockGroupConfigurationStrategy configurationStrategy, BlockFactory factory, IBlockColor color)
        {
            var _positions = configurationStrategy.GetPositions();

            foreach (var position in _positions)
            {
                var block = factory.CreateBlock(color) as IBlock;
                (this as IBlockGroup).AddBlock(block, position); //Add the block to the group

                
            }
        }


        ///////////////////////////////////////////////////////////////////
        /// IBlockGroup Interface
        ///////////////////////////////////////////////////////////////////
        void IBlockGroup.DestroyBlockGroup()
        {
            throw new NotImplementedException();
        }
        void IBlockGroup.ReleaseBlock(IBlock block)
        {
            if(blocks.Contains(block) == false)
                return;

            blocks.Remove(block);
            _positionsDeltaMap.Remove(block);
            (block as MonoBehaviour).transform.SetParent(null, false);
            (block as IGravity).OnBottomContact -= (this as IGravity).TriggerBottomReahed;
            (block as IBlock).SetParent(null);

             (this as IGravity).CheckIfFloating();

        }
        void IBlockGroup.AddBlock(IBlock block, GridPosition delta)
        {
            (block as MonoBehaviour).transform.SetParent(this.transform, false);
            (block as IGravity).OnBottomContact += (this as IGravity).TriggerBottomReahed;
            (block as IBlock).SetParent(this);
            this.blocks.Add(block);
            _positionsDeltaMap.Add(block, delta);
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
        void IBlockGroup.SetColor(IBlockColor color)
        {
            foreach (var block in blocks)
            {
                block.SetColor(color);
            }
        }



        ///////////////////////////////////////////////////////////////////
        /// ITakeBlockCommand Interface
        ///////////////////////////////////////////////////////////////////
        void ITakeBlockCommand.Place(Grid<BlockNode> colorGrid, GridPosition position)
        {

            foreach (var block in blocks)
            {
                var delta = _positionsDeltaMap[block];
                var newPosition = position + delta;
                block.Place(colorGrid, newPosition);
            }

            this.OnNeedGravity?.Invoke();
            this.OnPositionUpdated?.Invoke(position);
            ignoreOtherBottomReachedEvents = false;
        }
        void ITakeBlockCommand.Move(GridPosition direction)
        {
            if(canFall == false)
                return;

            Debug.Log($"***Moving BlockGroup: {this} Direction: {direction}");
            foreach (var block in blocks)
            {
                block.Move(direction);
            }

            ignoreOtherBottomReachedEvents = false;
        }
        void ITakeBlockCommand.Rotate(GridPosition delta)
        {
            if(canTakeCommands == false)
                return;

            foreach (var block in blocks)
            {
                var currentDelta = _positionsDeltaMap[block];
                var targetDelta = GetDeltaPosition(currentDelta, delta);

                var rotationPosition = targetDelta - currentDelta;

                block.Rotate(rotationPosition);
                _positionsDeltaMap[block] = targetDelta;

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
                    Debug.Log($"Grid Position: {block.GetGridPosition()}");
                    Debug.Log($"TargetPositon: {targetPosition}");
                    Debug.Log($"Invalid Move for Block: {block}");
                    return false;
                }
            }
            return true;
        }
        bool ITakeBlockCommand.CheckForValidRotation(GridPosition delta)
        {
            foreach (var block in blocks)
            {

                var currentDelta = _positionsDeltaMap[block];
                var targetDelta = GetDeltaPosition(currentDelta, delta);

                var rotationPosition = targetDelta - currentDelta;

                Debug.Log($"CurrentDelta: {currentDelta}");
                Debug.Log($"TargetDelta: {targetDelta}");
                Debug.Log($"RotationPosition: {rotationPosition}");
                    

                if (_positionsDeltaMap.ContainsValue(targetDelta)) //Check if the target position is occupied by another block in the group
                    continue;


                var isValid = block.CheckForValidRotation(rotationPosition);

                if (!isValid)
                {
                    Debug.Log($"Invalid Rotation for Block: {block}");
                    return false;
                }
            }
            return true;
        }

        List<IBlock> ITakeBlockCommand.GetBlocks()
        {
            return blocks;
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
            _actionQueue.Enqueue(action);
            CheckActionQueue();
        }



        ///////////////////////////////////////////////////////////////////
        /// IGravity Interface
        ///////////////////////////////////////////////////////////////////
        void IGravity.TriggerBottomReahed()
        {

            if (ignoreOtherBottomReachedEvents)
                return;


            ignoreOtherBottomReachedEvents = true;
            canTakeCommands = false;
            canFall = false;
            OnBottomContact?.Invoke();

            this.OnMergeCheckTriggered?.Invoke();
        }
        bool IGravity.CheckIfFloating()
        {
            var isGroupFloating = true;
            foreach (var block in blocks)
            {
                var isBlockFloating = (block as IGravity).CheckIfFloating();

                if (!isBlockFloating)
                {
                    isGroupFloating = false;
                    break;
                }
            }

            if (isGroupFloating)
            {
                canFall = true;
                canTakeCommands = false;
                this.OnNeedGravity?.Invoke();
            }

            Debug.Log($"IsGroupFloating: {isGroupFloating}");
            return isGroupFloating;
        }



        ///////////////////////////////////////////////////////////////////
        /// IEntity Interface
        ///////////////////////////////////////////////////////////////////
        void IEntity.SetGridNode(INode node)
        {
            //Noop
        }



        ///////////////////////////////////////////////////////////////////
        /// Private Helpers
        ///////////////////////////////////////////////////////////////////
        async void CheckActionQueue()
        {
            if (executingAction)
                return;

            if (_actionQueue.Count > 0)
            {
                Debug.Log($"Executing action queue. {canFall} {this}");
                var action = _actionQueue.Dequeue();
                executingAction = true;
                await action?.Invoke();
                executingAction = false;
                CheckActionQueue();
            }
        }

        private GridPosition GetDeltaPosition(GridPosition position, GridPosition deltaPosition)
        {
            return new GridPosition(position.y * deltaPosition.y, position.x * deltaPosition.x);
        }


        
    }
}