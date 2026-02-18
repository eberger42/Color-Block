using Assets.Scripts.Blocks.commands;
using Assets.Scripts.Blocks.components.colors;
using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.General.interfaces;
using Assets.Scripts.Grid.components;
using Assets.Scripts.Grid.interfaces;
using Assets.Scripts.Player.Interfaces;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Blocks.components
{
    public class ColorBlock : TakeBlockCommandMonobehaviour, IBlock, IGravity, ITriggerSpawn, IPlayerControlled
    {
        public bool DebugFlag = false;
        //Events
        private event Action<GridPosition> _onMoveDirection;
        private event Action<bool> _onEnableGravity;
        private event Action _onTriggerGravity;
        private event Action<IBlockColor> _onColorUpdated;
        private event Action<IEntity> _onEntityDestroyed;
        private event Action _onTriggerSpawn;

        //Fields
        private GridPosition gridPosition;
        [SerializeReference]
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
            this.parent = parent;
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
        bool IBlock.DoColorsMatch(IBlock block)
        {
            var blockColor = (block as ColorBlock).Color;
            return this.Color.Equals(blockColor);
        }
        bool IBlock.CheckMergeCompatability(IBlock block)
        {
            return Color.CanCombine(block.Color);
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
            Debug.Log($"Destroyed: {this}");
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

                var didMerge = AttemptMerge(block);


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
            return $"ColorBlock[{Color.GetColorRank()}] at ({gridPosition.x}, {gridPosition.y})";
        }
    }
}