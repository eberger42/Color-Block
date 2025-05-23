using Assets.Scripts.Blocks.commands;
using Assets.Scripts.Blocks.components;
using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.Grid.components;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {

        private CommandManager commandManager;
        private BlockManager blockManager;
        private ColorGridManager colorGridManager;

        private ITakeBlockCommand _target;

        private void Awake()
        {
            commandManager = new CommandManager();
            colorGridManager = FindFirstObjectByType<ColorGridManager>();
        }

        private void Start()
        {
            blockManager = BlockManager.Instance;
            blockManager.OnTargetCreated += OnBlockCreated;
        }

        private void OnDestroy()
        {
            blockManager.OnTargetCreated -= OnBlockCreated;
        }

        private void Update()
        {

            if(commandManager.IsExecuting)
                return;

            float vertical = Mathf.Round(Input.GetAxisRaw("Vertical"));
            float horizontal = Mathf.Round(Input.GetAxisRaw("Horizontal"));

            if (vertical == -1)
            {
                MoveTargetEntity(new Vector2(0, vertical));
            }

            if(horizontal != 0)
            {
                MoveTargetEntity(new Vector2(horizontal, 0));
            }

        }

        private async void MoveTargetEntity(Vector2 direction)
        {

            var gridDirection = new GridPosition((int)direction.x, (int)direction.y);
            var isValidMove = _target.CheckForValidMove(gridDirection);

            if(!isValidMove)
            {
                Debug.Log("Invalid move");
                if(gridDirection.y == -1)
                {
                    (_target as IGravity).TriggerBottomReahed();
                }
                return;
            }

            var moveBlockCommandConfigurer = new MoveBlockCommandConfigurer(gridDirection);
            var command = new CommandManager.CommandBuilder().AddCommand<MoveBlockCommand>(_target, moveBlockCommandConfigurer).Build();
            await commandManager.ExecuteCommands(command);
        }


        private void OnBlockCreated(ITakeBlockCommand target)
        {
            SetTargetEntity(target);
        }

        private void SetTargetEntity(ITakeBlockCommand target)
        {
            _target = target;
        }





    }
}