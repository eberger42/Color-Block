using Assets.Scripts.Blocks.components;
using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.Grid.components;
using Assets.Scripts.Player.Interfaces;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Blocks.commands
{
    public class PlaceBlockCommand : BlockCommand
    {
        private GridPosition position;
        private Grid<BlockNode> colorGrid;

        public PlaceBlockCommand(ITakeBlockCommand target) : base(target) { }

        public void SetDirection(GridPosition position)
        {
            this.position = position;
        }

        public void SetGrid(Grid<BlockNode> colorGrid)
        {
            this.colorGrid = colorGrid;
        }

        public override async Task Execute()
        {
            if (_target.CanTakeCommand(this) == false)
                return;

            //Debug.Log($"Executing PlaceBlockCommand with target: {_target}");
            
            _target.Place(colorGrid, position);
            await Task.Delay(100);
        }
    }

    public class PlaceBlockCommandConfigurer : IConfigureCommand
    {
        private GridPosition position;
        private Grid<BlockNode> colorGrid;

        public PlaceBlockCommandConfigurer(Grid<BlockNode>  colorGrid, GridPosition position)
        {
            this.position = position;
            this.colorGrid = colorGrid;
        }

        public void Configure(ICommand command)
        {
            try
            {
                if (!(command is PlaceBlockCommand))
                    throw new System.Exception("Command is not a PlaceBlockCommand");

                (command as PlaceBlockCommand).SetDirection(position);
                (command as PlaceBlockCommand).SetGrid(colorGrid);
                
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error configuring command: {e.Message}");
            }
        }

    }
}