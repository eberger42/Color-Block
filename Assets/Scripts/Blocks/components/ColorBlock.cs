using Assets.Scripts.Blocks.commands;
using Assets.Scripts.Blocks.components.colors;
using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.Blocks.ux;
using Assets.Scripts.General.interfaces;
using Assets.Scripts.Grid.components;
using Assets.Scripts.Grid.interfaces;
using Assets.Scripts.Player.Interfaces;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Blocks.components
{
    public class ColorBlock : TakeBlockCommandMonobehaviour, IBlock, IGravity, IPlayerControlled
    {
        public bool DebugFlag = false;

        //Events
        private event Action<GridPosition> _onMoveDirection;
        private event Action<bool> _onEnableGravity;
        private event Action _onTriggerGravity;
        private event Action<BlockColorUpdateEventArgs> _onColorUpdated;
        private event Action<IEntity> _onEntityDestroyed;
        private event Action _onPlayerControlCompleted;
        private event Action _onBlockRemoved;

        //Fields
        private GridPosition gridPosition;
        [SerializeReference] private INode node;
        private IBlockGroup parent;
        private ColorBlockUX _colorBlockUX;

        //Properties
        public IBlockColor CurrentColor { get; private set; }


        //Unity Lifecycle
        private void Awake()
        {

            _colorBlockUX = GetComponent<ColorBlockUX>();

            _colorBlockUX.OnRemovalAnimationComplete += ColorBlockUX_OnRemovalAnimationComplete;

        }

        private void OnDestroy()
        {
            if(_colorBlockUX != null)
                _colorBlockUX.OnRemovalAnimationComplete -= ColorBlockUX_OnRemovalAnimationComplete;
        }


        public bool AttemptMerge(ColorBlock colorBlock, GridPosition direction)
        {
            if (!CurrentColor.CanCombine(colorBlock.CurrentColor))
                return false;


            (this as IBlock).MergeColor(colorBlock.CurrentColor, direction);


            return true;
        }

        public ColorRank GetColorRank()
        {
            return CurrentColor.GetColorRank();
        }

        public void SetWorldPosition(Vector2 position)
        {
            this.transform.position = position;
        }

        //Combinte These two, maybe static method to convert grid to world
        public void SetGridPosition(GridPosition position)
        {
            this.gridPosition = position;
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
        bool IGravity.CheckIfFloating()
        {
            var isFloating = CheckForValidMove(GridPosition.Down);
            return isFloating;
        }

        void IGravity.SetEnable(bool state)
        {
            if (state == false)
            {
                _onEnableGravity?.Invoke(false);

            }
            else
            {
                _onEnableGravity?.Invoke(true);

            }

        }

        void IGravity.Trigger()
        {
            if(DebugFlag)
                Debug.Log($"Gravity Triggered for {this}");
            _onTriggerGravity?.Invoke();
        }

        /////////////////////////////////////////////////////////////////
        /// IBlock Interface
        /////////////////////////////////////////////////////////////////

        //Events
        event Action<BlockColorUpdateEventArgs> IBlock.OnColorUpdated
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

        event Action IBlock.OnBlockRemoved
        {
            add
            {
                _onBlockRemoved += value;
            }
            remove
            {
                _onBlockRemoved -= value;
            }
        }

        //Functions
        void IBlock.SetParent(IBlockGroup parent)
        {
            this.parent = parent;
        }
        IBlockGroup IBlock.GetParent()
        {
            return parent;
        }
        void IBlock.SetColor(IBlockColor newColor)
        {
            var updateColorEventArgs = new BlockColorUpdateEventArgs(newColor, newColor, GridPosition.Zero);
            this.CurrentColor = newColor;
            _onColorUpdated?.Invoke(updateColorEventArgs);

        }
        void IBlock.MergeColor(IBlockColor incomingColor, GridPosition direction)
        {

            var newColor = CurrentColor.GetCombineColor(incomingColor);
            var updateColorEventArgs = new BlockColorUpdateEventArgs(newColor, incomingColor, direction);

            this.CurrentColor = newColor;
            Debug.Log($"Merged color to {newColor.GetColorType()}");
            _onColorUpdated?.Invoke(updateColorEventArgs);
        }
        bool IBlock.DoColorsMatch(IBlock block)
        {
            var blockColor = (block as ColorBlock).CurrentColor;
            return this.CurrentColor.Equals(blockColor);
        }
        bool IBlock.CheckMergeCompatability(IBlock block)
        {
            return CurrentColor.CanCombine(block.CurrentColor);
        }

        void IBlock.Remove()
        {
            _onBlockRemoved?.Invoke();
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

        event Action<IEntity> IEntity.OnEntityDestroyed
        {
            add
            {
                _onEntityDestroyed += value;
            }
            remove
            {
                _onEntityDestroyed -= value;
            }
        }

        void IEntity.Destroy()
        {
            node.ClearNodeData(this);
            _onEntityDestroyed?.Invoke(this);
            Destroy(this.gameObject);
        }


        /////////////////////////////////////////////////////////////////
        /// ITakeBlockCommand Interface
        /////////////////////////////////////////////////////////////////

        public override void Place(Grid<BlockNode> colorGrid, GridPosition position)
        {
            colorGrid.SetNodeData(position.x, position.y, this);
        }
        public override void Gravity()
        {
            var postition = GridPosition.Down;
            Move(postition);
        }
        public override void Move(GridPosition direction)
        {
            var canFall = CanTakeCommand(typeof(GravityBlockCommand));
            if (canFall == false)
                return;

            var neigborNode = node.GetNeighbor(direction);
            

            if(neigborNode.GetData() is ColorBlock block && parent.DoesContainBlock(block) == false)
            {

                var didMerge = AttemptMerge(block, direction);


                if (didMerge)
                {
                    node.ClearNodeData(this);
                    (block as IEntity).Destroy();
                    neigborNode.SetNodeData(this);
                    _onMoveDirection?.Invoke(direction);
                    (neigborNode as BlockNode).ReportColorChange();

                }
            }
            else
            {
                node.ClearNodeData(this);
                neigborNode.SetNodeData(this);
                _onMoveDirection?.Invoke(direction);

            }


            
        }
        public override void Rotate(GridPosition delta)
        {
            //Singulare Block, so no rotation
        }
        public override bool CheckForValidMove(GridPosition direction)
        {
            var isValidMove = false;

            if(node.GetNeighbor(direction) is INode neighbor)
            {
                if(neighbor.GetData() is IBlock block)
                {
                    isValidMove = block.CheckMergeCompatability(this);
                }
                else
                {
                    isValidMove = true;
                }
            }

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
            if (nodeEvent is NodeDataRemoved removedDataEvent)
            {
                //Ignoring events from the same parent as they move together and would not 
                //cause a gravity check.
                if(removedDataEvent.RemovedData is IBlock blockData)
                {
                    if (parent == null)
                        return;
                    if(parent.DoesContainBlock(blockData))
                        return;
                }

                var sender = nodeEvent.GetSender() as INode;

                var vectorTo = Node.VectorTo(node, sender);

                if (vectorTo == GridPosition.Down)
                {
                    (this as IGravity).SetEnable(true);
                }
            }
        }

        ///////////////////////////////////////////////////////////////////
        /// IPlayerControlled Interface
        ///////////////////////////////////////////////////////////////////


        event Action IPlayerControlled.OnPlayerControlCompleted
        {
            add
            {
                _onPlayerControlCompleted += value;
            }
            remove
            {
                _onPlayerControlCompleted -= value;
            }
        }

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
        /// ITick Interface
        ///////////////////////////////////////////////////////////////////
        void ITick.Tick()
        {
            (this as IGravity).Trigger();
        }




        ///////////////////////////////////////////////////////////////////
        /// Private Helpers
        ///////////////////////////////////////////////////////////////////


        public override string ToString()
        {
            return $"ColorBlock[{CurrentColor.GetColorRank()}] at ({gridPosition.x}, {gridPosition.y})";
        }

        //Event Callbacks

        private void ColorBlockUX_OnRemovalAnimationComplete()
        {
            (this as IEntity).Destroy();
        }

    }
}