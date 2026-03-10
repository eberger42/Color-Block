using Assets.Scripts.Blocks.commands;
using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.General;
using Assets.Scripts.General.interfaces;
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

        private CancellationTokenSource gravityTokenSource;


        private void Awake()
        {
            commandManager = new CommandManager();

            _targetEntity = GetComponent<ITakeBlockCommand>();

            (_targetEntity as IGravity).OnEnableGravity += HandleGravityEnabledState;
            (_targetEntity as IGravity).OnTriggerGravity += TriggerGravity;



        }

        private void Start()
        {

        }
        private void OnDisable()
        {
            (_targetEntity as IGravity).OnEnableGravity -= HandleGravityEnabledState;
            gravityTokenSource?.Cancel();
        }

        void TriggerGravity()
        {
            if (Enabled)
            {
                if (commandManager.IsExecuting)
                    return;
                gravityTokenSource = new CancellationTokenSource();

                GravityCalculation();
                return;
            }
            else
            {
                commandManager.Cancel();
                gravityTokenSource?.Cancel();
            }
        }

        private void HandleGravityEnabledState(bool state)
        {
            if(this.Enabled == state)
            {
                return;
            }
            this.Enabled = state;
            
        }
        
        private async void GravityCalculation()
        {

            if (!Enabled)
                return;
            var moveBlockCommandConfigurer = new GravityBlockCommandConfigurer(GridPosition.Down);
            var command = new CommandManager.CommandBuilder().AddCommand<GravityBlockCommand>(_targetEntity, moveBlockCommandConfigurer).Build();
            await commandManager.ExecuteCommands(command);

        }

    }
}