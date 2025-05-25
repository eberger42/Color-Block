using Assets.Scripts.Blocks.commands;
using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.Grid.components;
using Assets.Scripts.Player.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Blocks.components
{

    [RequireComponent(typeof(ITakeBlockCommand))]
    public class BlockGravity : MonoBehaviour
    {

        private CommandManager commandManager;
        private ITakeBlockCommand _targetEntity;

        private List<ICommand> _commandBuffer = new List<ICommand>(); 
        private CancellationTokenSource gravityTokenSource;


        private void Awake()
        {
            commandManager = new CommandManager();
            _targetEntity = GetComponent<ITakeBlockCommand>();

            _targetEntity.OnPositionUpdated += EntityPositionUpdated;


        }

        private void Start()
        {

        }
        private void OnDisable()
        {
            gravityTokenSource?.Cancel();
        }

        private void EntityPositionUpdated(GridPosition position)
        {
            if (commandManager.IsExecuting)
                return;
            gravityTokenSource = new CancellationTokenSource();
            GravityCalculation();
        }
        
        private async void GravityCalculation()
        {

            if (gravityTokenSource.IsCancellationRequested)
                return;

            var gridDirection = new GridPosition(0, -1);
            var isValidMove = _targetEntity.CheckForValidMove(gridDirection);

            if (!isValidMove)
            {
                Debug.Log("Finished Falling");
                (_targetEntity as IGravity).TriggerBottomReahed();
                return;
            }

            var moveBlockCommandConfigurer = new GravityBlockCommandConfigurer(gridDirection);
            var command = new CommandManager.CommandBuilder().AddCommand<GravityBlockCommand>(_targetEntity, moveBlockCommandConfigurer).Build();
            await commandManager.ExecuteCommands(command);


            GravityCalculation();
        }
    }
}