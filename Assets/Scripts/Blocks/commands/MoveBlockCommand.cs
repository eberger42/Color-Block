using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.Player.Interfaces;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Blocks.commands
{
    public class MoveBlockCommand : BlockCommand
    {
        private GridPosition _direction;

        public MoveBlockCommand(IEntity entity) : base(entity) { }

        public void SetDirection(GridPosition direction)
        {
            _direction = direction;
        }

        public override async Task Execute()
        {
            _entity.MoveDirection(_direction);
            await Task.Delay(100);
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