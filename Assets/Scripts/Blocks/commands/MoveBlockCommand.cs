using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.Player.Interfaces;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Blocks.commands
{
    
    public class MoveBlockCommand : BlockCommand
    {
        private GridPosition _direction;

        public MoveBlockCommand(ITakeBlockCommand target) : base(target) { }

        public void SetDirection(GridPosition direction)
        {
            _direction = direction;
        }

        public override async Task Execute()
        {

            Func<Task> task = async () =>
            {
                if (_target.CanTakeCommand(this) == false)
                    return;

                var isValidMove = _target.CheckForValidMove(_direction);
                if (!isValidMove)
                    return;

                _target.Move(_direction);
                await Task.Delay(5);
            };
            
            _target.AddActionCommand(task);

            await Task.Delay(75);
        }
    }

    public class MoveBlockCommandConfigurer : IConfigureCommand
    {
        private GridPosition _direction;

        public MoveBlockCommandConfigurer(GridPosition direction)
        {
            _direction = direction;
        }

        public void Configure(ICommand command)
        {
            try
            {
                if (!(command is MoveBlockCommand))
                    throw new System.Exception("Command is not a MoveBlockCommand");

                (command as MoveBlockCommand).SetDirection(_direction);
                
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error configuring command: {e.Message}");
            }
        }

    }
}