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
using static Unity.Collections.AllocatorManager;
using static UnityEngine.GraphicsBuffer;

namespace Assets.Scripts.Blocks.components
{
    /// <summary>
    /// The task Queue is used to execute the commands in order, one at a time. Preventing blocks phasing through each other
    /// </summary>
    public class ColorBlockGroupController : TakeBlockCommandMonobehaviour, IBlockGroup, IGravity, ITriggerSpawn, IPlayerControlled
    {
        private event Action<bool> _onEnableGravity;
        private event Action _onTriggerGravity;
        private event Action _onMergeCheckTriggered;
        private event Action _onTriggerSpawn;
        private event Action<GridPosition> _onPositionUpdated;

        //Block Positioning Helpers
        private Dictionary<IBlock,GridPosition> _positionsDeltaMap = new Dictionary<IBlock, GridPosition>();

        //Grouped elements
        private List<IBlock> blocks = new List<IBlock>();

        //Flags
        private bool canTriggeredSpawn = false;
        private bool isGrounded = false;
        private bool waitForInit = true;

        public void Initialize(IBlockGroupConfigurationStrategy configurationStrategy, BlockFactory factory, IBlockColor color)
        {
            var _positions = configurationStrategy.GetPositions();

            foreach (var position in _positions)
            {
                var block = factory.CreateBlock(color) as IBlock;
                (this as IBlockGroup).AddBlock(block, position); //Add the block to the group

            }
            waitForInit = false;
        }

        public void Initialize(List<IBlock> blocks)
        {
            var firstPosition = blocks[0].GetGridPosition();

            foreach (var block in blocks)
            {
                var delta = block.GetGridPosition() - firstPosition;
                (this as IBlockGroup).AddBlock(block, delta); //Add the block to the group
            }
            waitForInit = false;
            canTriggeredSpawn = false;

            var isFloating = (this as IGravity).CheckIfFloating(); //Check if the group is floating and set the gravity accordingly

            if (!isFloating)
            {
                (this as IGravity).SetEnable(false);
                return;
            }

            (this as IGravity).SetEnable(true); //Enable gravity if the group is floating

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

            while(blocks.Count > 0)
                (this as IBlockGroup).ReleaseBlock(blocks[0]);


            SpawnCheck();

        }

        void IBlockGroup.ReleaseBlock(IBlock block)
        {
            if(blocks.Contains(block) == false)
                return;

            blocks.Remove(block);
            _positionsDeltaMap.Remove(block);
            (block as MonoBehaviour).transform.SetParent(null, false);
            (block as IGravity).OnEnableGravity -= HandleChildBlockGravityUpdate;
            (block as IGravity).OnTriggerGravity -= (this as IGravity).Trigger;
            (block as IBlock).SetParent(null);


            BlockManager.Instance.AssignBlockGroupToBlocks(new List<IBlock> { block });

        }
        
        void IBlockGroup.AddBlock(IBlock block, GridPosition delta)
        {
            (block as MonoBehaviour).transform.SetParent(this.transform, false);
            (block as IGravity).OnEnableGravity += HandleChildBlockGravityUpdate;
            (block as IGravity).OnTriggerGravity += (this as IGravity).Trigger;
            (block as IBlock).SetParent(this);
            (block as IBlock).OnColorUpdated += HandleChildBlockColorUpdated;
            this.blocks.Add(block);
            _positionsDeltaMap.Add(block, delta);
        }
        
        void IBlockGroup.SetColor(IBlockColor color)
        {
            foreach (var block in blocks)
            {
                block.SetColor(color);
            }
        }

        bool IBlockGroup.DoesContainBlock(IBlock block)
        {
            if(blocks.Count == 0) return false;

            var containsBlock = this.blocks.Contains(block);
            return containsBlock;
        }


        ///////////////////////////////////////////////////////////////////
        /// ITakeBlockCommand Interface
        ///////////////////////////////////////////////////////////////////
        public override void Place(Grid<BlockNode> colorGrid, GridPosition position)
        {
            var isGroupFloating = true;

            foreach (var block in blocks)
            {
                var delta = _positionsDeltaMap[block];
                var newPosition = position + delta;
                (block as ITakeBlockCommand).Place(colorGrid, newPosition);

                var isFloating = CheckSingleBlockIfFloating(block);

                if (!isFloating)
                {
                    isGroupFloating = false;
                }
            }

            this._onPositionUpdated?.Invoke(position);

            (this as IGravity).SetEnable(isGroupFloating);
            
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


            if(direction != GridPosition.Down) //If the direction is not down, we don't need to check for floating blocks
                return;

            var isGroupFloating = true;
            foreach(var block in blocks)
            {
                var isFloating = CheckSingleBlockIfFloating(block);

                if (!isFloating)
                {
                    isGroupFloating = false;
                }
            }
            (this as IGravity).SetEnable(isGroupFloating);
        }
        public override void Gravity()
        {
            var direction = GridPosition.Down;
            var canFall = CanTakeCommand(typeof(GravityBlockCommand));
            if (canFall == false)
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

                (block as ITakeBlockCommand).Move(rotationPosition);
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

        event Action IGravity.OnTriggerGravity
        {
            add
            {
                _onTriggerGravity += value;
            }
            remove
            {
                _onTriggerGravity -= value;
            }
        }

        //Functions
        void IGravity.SetEnable(bool state)
        {
            if(waitForInit)
                return;

            if(state == false)
            {

                SpawnCheck();

                RemoveCommandFromFilter(typeof(GravityBlockCommand));

                isGrounded = true;
                _onEnableGravity?.Invoke(false);
                CheckBlocksForMerger();

            }
            else
            {
                AddCommandToFilter(typeof(GravityBlockCommand));
                _onEnableGravity?.Invoke(true);
                isGrounded = false;
            }

        }

        bool IGravity.CheckIfFloating()
        {
            var isGroupFloating = true;
            foreach (var block in blocks)
            {
                var isBlockFloating = CheckSingleBlockIfFloating(block);

                if (!isBlockFloating)
                {
                    isGroupFloating = false;
                    break;
                }
            }

            return isGroupFloating;
        }

        void IGravity.Trigger()
        {
            _onTriggerGravity?.Invoke();
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

        private bool CheckSingleBlockIfFloating(IBlock block)
        {
            var targetPosition = _positionsDeltaMap[block] + GridPosition.Down;

            if (_positionsDeltaMap.ContainsValue(targetPosition)) //Check if the target position is occupied by another block in the group
                return true;

            var isFloating = (block as IGravity).CheckIfFloating();

            return isFloating;
        }

        private void CheckBlocksForMerger()
        {

            if(isGrounded == false)
            {
                return;
            }

            var blockToRemove = new List<IBlock>();
            var tempParentBlocks = new List<IBlock>(blocks);
            foreach (var block in tempParentBlocks)
            {
                var canMerge = (block as IBlock).CheckNeighborsForMerge();

                if(canMerge)
                {
                    blockToRemove.Add(block);
                }
            }

            if(blockToRemove.Count == 0)
            {
                Debug.Log("No blocks to remove.");
                return;
            }

            foreach (var block in blockToRemove)
            {
                (block as IEntity).Destroy();
                blocks.Remove(block);
            }
            while(blocks.Count > 0)
            {
                var block = blocks[0];
                (this as IBlockGroup).ReleaseBlock(block);

                if(blocks.Contains(block) == false) //If the block was removed from the group, we can continue
                    continue;

                blocks.Remove(block);
            }

            Destroy(this.gameObject);
        }

        private void HandleChildBlockGravityUpdate(bool isFloating)
        {

            var groupIsFloating = (this as IGravity).CheckIfFloating();

            if (groupIsFloating)
            {
                (this as IGravity).SetEnable(true);
            }
            else
            {
                (this as IGravity).SetEnable(false);
            }
            
        }

        private void HandleChildBlockColorUpdated(IBlockColor blockColor)
        {
            if(blocks.Count > 1)
            {
                (this as IBlockGroup).Disband();
            }
        }

        private void SpawnCheck()
        {
            if (canTriggeredSpawn)
            {
                Debug.LogWarning("Spawning Blocks.");
                (this as ITriggerSpawn).SetEnabled(false);
                _onTriggerSpawn?.Invoke();
            }
        }
    }
}