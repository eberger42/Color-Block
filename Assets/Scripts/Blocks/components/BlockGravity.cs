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
        public bool Enabled { get; private set; } = false;

        private CommandManager commandManager;
        private ITakeBlockCommand _targetEntity;

        private List<ICommand> _commandBuffer = new List<ICommand>(); 
        private CancellationTokenSource gravityTokenSource;


        private void Awake()
        {
            commandManager = new CommandManager();
            _targetEntity = GetComponent<ITakeBlockCommand>();

            (_targetEntity as IGravity).OnEnableGravity += HandleGravityEnabledState;


        }

        private void Start()
        {

        }
        private void OnDisable()
        {
            (_targetEntity as IGravity).OnEnableGravity -= HandleGravityEnabledState;
            gravityTokenSource?.Cancel();
        }

        private void HandleGravityEnabledState(bool state)
        {

            if(this.Enabled == state)
                return;

            this.Enabled = state;


            if(Enabled)
            {
                if (commandManager.IsExecuting)
                    return;

                gravityTokenSource = new CancellationTokenSource();

                GravityCalculation();
                return;
            }
            else
            {
                gravityTokenSource?.Cancel();
            }
            
        }
        
        private async void GravityCalculation()
        {
            if (gravityTokenSource.IsCancellationRequested)
                return;

            if (!Enabled)
                return;

            var moveBlockCommandConfigurer = new GravityBlockCommandConfigurer(GridPosition.Down);
            var command = new CommandManager.CommandBuilder().AddCommand<GravityBlockCommand>(_targetEntity, moveBlockCommandConfigurer).Build();
            await commandManager.ExecuteCommands(command);

            GravityCalculation();
        }
    }
}