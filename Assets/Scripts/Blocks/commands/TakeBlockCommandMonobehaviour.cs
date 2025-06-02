using Assets.Scripts.Blocks.components;
using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.Grid.components;
using Assets.Scripts.Player.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Blocks.commands
{
    public abstract class TakeBlockCommandMonobehaviour : MonoBehaviour, ITakeBlockCommand
    {

        public static List<Type> PlayerCommands { get; } = new List<Type> { typeof(MoveBlockCommand), typeof(PlaceBlockCommand), typeof(RotateBlockCommand) };

        //Command/Action Queue
        protected Queue<Func<Task>> _actionQueue = new Queue<Func<Task>>();
        protected List<Type> _commandFilter = new List<Type>();

        //Flags
        public bool ExecutingAction { get; protected set; } = false;


        /////////////////////////////////////////////////////////////////
        /// ITakeBlockCommand Interface
        /////////////////////////////////////////////////////////////////
       
        //Abstract
        public abstract void Move(GridPosition direction);
        public abstract void Gravity();
        public abstract void Place(Grid<BlockNode> colorGrid, GridPosition position);
        public abstract void Rotate(GridPosition delta);
        public abstract bool CheckForValidMove(GridPosition direction);
        public abstract bool CheckForValidRotation(GridPosition position);

        //Virtual
        public virtual bool CanTakeCommand(ICommand command)
        {

            var commandType = command.GetType();

            var isSubTypeBlockCommand = typeof(BlockCommand).IsAssignableFrom(commandType);

            if (!isSubTypeBlockCommand)
            {
                throw new ArgumentException($"Command type {commandType} is not a valid BlockCommand type.");
            }

            var isCommandAllowed = _commandFilter.Count == 0 || _commandFilter.Contains(commandType);

            return isCommandAllowed;
        }
        public virtual bool CanTakeCommand(Type commandType)
        {
            var isSubTypeBlockCommand = typeof(BlockCommand).IsAssignableFrom(commandType);

            if (!isSubTypeBlockCommand)
            {
                throw new ArgumentException($"Command type {commandType} is not a valid BlockCommand type.");
            }

            var isCommandAllowed = _commandFilter.Count == 0 || _commandFilter.Contains(commandType);

            return isCommandAllowed;
        }
        public virtual void AddCommandToFilter(BlockCommand blockCommand)
        {
            if (blockCommand == null)
                throw new ArgumentNullException(nameof(blockCommand), "BlockCommand cannot be null.");

            var commandType = blockCommand.GetType();

            if(_commandFilter.Contains(commandType))
            {
                Debug.LogWarning($"Command type {commandType} is already in the filter. No action taken.");
                return;
            }

            _commandFilter.Add(commandType);
        }
        public virtual void RemoveCommandFromFilter(BlockCommand blockCommand)
        {
            if (blockCommand == null)
                throw new ArgumentNullException(nameof(blockCommand), "BlockCommand cannot be null.");

            var commandType = blockCommand.GetType();

            if (!_commandFilter.Contains(commandType))
            {
                Debug.LogWarning($"Command type {commandType} is not in the filter. No action taken.");
                return;
            }

            _commandFilter.Remove(commandType);
        }
        public virtual void AddCommandToFilter(Type blockCommandType)
        {
            if (blockCommandType == null)
                throw new ArgumentNullException(nameof(blockCommandType), "BlockCommandType cannot be null.");

            if (_commandFilter.Contains(blockCommandType))
            {
                //Debug.LogWarning($"Command type {blockCommandType} is already in the filter. No action taken.");
                return;
            }

            _commandFilter.Add(blockCommandType);
        }
        public virtual void RemoveCommandFromFilter(Type blockCommandType)
        {
            if (blockCommandType == null)
                throw new ArgumentNullException(nameof(blockCommandType), "BlockCommand cannot be null.");

            if (!_commandFilter.Contains(blockCommandType))
            {
                //Debug.LogWarning($"Command type {blockCommandType} is not in the filter. No action taken.");
                return;
            }

            _commandFilter.Remove(blockCommandType);
        }

        public virtual void AddActionCommand(Func<Task> action)
        {
            _actionQueue.Enqueue(action);
            CheckActionQueue();
        }

        ///////////////////////////////////////////////////////////////////
        /// Private Helpers
        ///////////////////////////////////////////////////////////////////
        protected async void CheckActionQueue()
        {
            if (ExecutingAction)
                return;

            if (_actionQueue.Count > 0)
            {
                var action = _actionQueue.Dequeue();
                ExecutingAction = true;
                await action?.Invoke();
                ExecutingAction = false;
                CheckActionQueue();
            }
        }

    }
}