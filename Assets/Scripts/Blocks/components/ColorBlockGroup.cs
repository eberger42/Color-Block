using Assets.Scripts.Blocks.commands;
using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.Grid.components;
using Assets.Scripts.Player.Interfaces;
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
    public class ColorBlockGroup : TakeBlockCommandMonobehaviour, IBlockGroup, IGravity, ITriggerSpawn, IPlayerControlled
    {
        private event Action<bool> _onEnableGravity;
        private event Action _onMergeCheckTriggered;
        private event Action _onTriggerSpawn;
        private event Action<GridPosition> _onPositionUpdated;

        //Block Positioning Helpers
        private Dictionary<IBlock,GridPosition> _positionsDeltaMap = new Dictionary<IBlock, GridPosition>();

        //Grouped elements
        private List<IBlock> blocks = new List<IBlock>();

        //Flags
        private bool canTriggeredSpawn = false;

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

        //Events
        event Action IBlockGroup.OnMergeCheckTriggered
        {
            add
            {
                _onMergeCheckTriggered += value;
            }
            remove
            {
                _onMergeCheckTriggered -= value;
            }
        }

        event Action<GridPosition> IBlockGroup.OnPositionUpdated
        {
            add 
            {                 
                _onPositionUpdated += value;
            }
            remove 
            {              
                _onPositionUpdated -= value;
            }
        }

        //Functions
        void IBlockGroup.Disband()
        {
            foreach(var block in blocks)
            {
                (block as MonoBehaviour).transform.SetParent(null, false);
                (block as IGravity).OnEnableGravity -= (this as IGravity).SetEnable;
                (block as IBlock).SetParent(null);
            }
        }

        void IBlockGroup.ReleaseBlock(IBlock block)
        {
            if(blocks.Contains(block) == false)
                return;

            blocks.Remove(block);
            _positionsDeltaMap.Remove(block);
            (block as MonoBehaviour).transform.SetParent(null, false);
            (block as IGravity).OnEnableGravity -= (this as IGravity).SetEnable;
            (block as IBlock).SetParent(null);

             (this as IGravity).CheckIfFloating();

        }
        
        void IBlockGroup.AddBlock(IBlock block, GridPosition delta)
        {
            (block as MonoBehaviour).transform.SetParent(this.transform, false);
            (block as IGravity).OnEnableGravity += (this as IGravity).SetEnable;
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
        public override void Place(Grid<BlockNode> colorGrid, GridPosition position)
        {

            foreach (var block in blocks)
            {
                var delta = _positionsDeltaMap[block];
                var newPosition = position + delta;
                (block as ITakeBlockCommand).Place(colorGrid, newPosition);
            }

            this._onPositionUpdated?.Invoke(position);

        }
        public override void Move(GridPosition direction)
        {
            var canFall = CanTakeCommand(typeof(GravityBlockCommand));
            if(canFall == false)
                return;

            foreach (var block in blocks)
            {
                (block as ITakeBlockCommand).Move(direction);
            }

        }
        public override void Rotate(GridPosition delta)
        {
            var canRotate = CanTakeCommand(typeof(RotateBlockCommand));
            if(canRotate == false)
                return;

            foreach (var block in blocks)
            {
                var currentDelta = _positionsDeltaMap[block];
                var targetDelta = GetDeltaPosition(currentDelta, delta);

                var rotationPosition = targetDelta - currentDelta;

                (block as ITakeBlockCommand).Rotate(rotationPosition);
                _positionsDeltaMap[block] = targetDelta;

            }
        }
        public override bool CheckForValidMove(GridPosition direction)
        {
            foreach (var block in blocks)
            {

                var targetPosition = _positionsDeltaMap[block] + direction;

                if(_positionsDeltaMap.ContainsValue(targetPosition)) //Check if the target position is occupied by another block in the group
                    continue;


                var isValid = (block as ITakeBlockCommand).CheckForValidMove(direction);

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
        public override bool CheckForValidRotation(GridPosition delta)
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


                var isValid = (block as ITakeBlockCommand).CheckForValidRotation(rotationPosition);

                if (!isValid)
                {
                    Debug.Log($"Invalid Rotation for Block: {block}");
                    return false;
                }
            }
            return true;
        }
        

        ///////////////////////////////////////////////////////////////////
        /// IGravity Interface
        ///////////////////////////////////////////////////////////////////

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
        void IGravity.SetEnable(bool state)
        {

            if(state == false)
            {

                if (canTriggeredSpawn)
                {
                    Debug.LogWarning("Gravity is already disabled. No action taken.");
                    (this as ITriggerSpawn).SetEnabled(false);
                    _onTriggerSpawn?.Invoke();
                }

                RemoveCommandFromFilter(typeof(GravityBlockCommand));

                _onEnableGravity?.Invoke(false);
                _onMergeCheckTriggered?.Invoke();

            }
            else
            {
                AddCommandToFilter(typeof(GravityBlockCommand));

                _onEnableGravity?.Invoke(true);
            }

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

            if (isGroupFloating && !CanTakeCommand(typeof(GravityBlockCommand)))
            {
                (this as IGravity).SetEnable(true);
            }

            Debug.Log($"IsGroupFloating: {isGroupFloating}");
            return isGroupFloating;
        }


        ///////////////////////////////////////////////////////////////////
        /// IGravity Interface
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
        /// ITriggerSpawn Interface
        ///////////////////////////////////////////////////////////////////
        void ITriggerSpawn.SetEnabled(bool state)
        {
            canTriggeredSpawn = state;
        }

        ///////////////////////////////////////////////////////////////////
        /// Private Helpers
        ///////////////////////////////////////////////////////////////////
        private GridPosition GetDeltaPosition(GridPosition position, GridPosition deltaPosition)
        {
            return new GridPosition(position.y * deltaPosition.y, position.x * deltaPosition.x);
        }

    }
}