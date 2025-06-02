using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.Player.Interfaces;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Blocks.commands
{
    public class GravityBlockCommand : BlockCommand
    {
        private GridPosition _direction;

        public GravityBlockCommand(ITakeBlockCommand target) : base(target) { }

        public void SetDirection(GridPosition direction)
        {
            _direction = direction;
        }

        public override async Task Execute()
        {

            Func<Task> task = async () =>
            {
                //Debug.Log($"Executing GravityBlockCommand with direction: {_target}");

                if (_target.CanTakeCommand(this) == false)
                    return;

                var isFloating = (_target as IGravity).CheckIfFloating();
                
                if(isFloating)
                {
                    _target.Gravity();
                    return;
                }
                else
                { 
                    (_target as IGravity).SetEnable(false);
                }

                await Task.Delay(50);
            };

            _target.AddActionCommand(task);
            await Task.Delay(500);


        }
    }

    public class GravityBlockCommandConfigurer : IConfigureCommand
    {
        private GridPosition _direction;

        public GravityBlockCommandConfigurer(GridPosition direction)
        {
            _direction = direction;
        }

        public void Configure(ICommand command)
        {
            try
            {
                if (!(command is GravityBlockCommand))
                    throw new System.Exception("Command is not a GravityBlockCommand");

                (command as GravityBlockCommand).SetDirection(_direction);

            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error configuring command: {e.Message}");
            }
        }

    }

}