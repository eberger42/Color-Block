using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.Player.Interfaces;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Blocks.commands
{
    
    public class RotateBlockCommand : BlockCommand
    {
        private GridPosition _rotationDelta;

        public RotateBlockCommand(ITakeBlockCommand target) : base(target) { }

        public void SetDirection(GridPosition direction)
        {
            _rotationDelta = direction;
        }

        public override async Task Execute()
        {
            Debug.Log($"Executing RotateBlockCommand with direction: {_rotationDelta}");
            Func<Task> task = async () =>
            {
                if (_target.CanTakeCommand(this) == false)
                    return;

                var isValidRotate = _target.CheckForValidRotation(_rotationDelta);
                if (!isValidRotate)
                    return;
                _target.Rotate(_rotationDelta);
                await Task.Delay(50);
            };
            

            _target.AddActionCommand(task);

            await Task.Delay(55);
        }
    }

    public class RotateBlockCommandConfigurer : IConfigureCommand
    {
        private GridPosition _rotationDelta;

        public RotateBlockCommandConfigurer(GridPosition rotationDelta)
        {
            _rotationDelta = rotationDelta;
        }

        public void Configure(ICommand command)
        {
            try
            {
                if (!(command is RotateBlockCommand))
                    throw new System.Exception("Command is not a RotateBlockCommand");

                (command as RotateBlockCommand).SetDirection(_rotationDelta);
                
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error configuring command: {e.Message}");
            }
        }

    }
}