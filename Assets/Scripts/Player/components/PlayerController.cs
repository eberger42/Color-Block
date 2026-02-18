using Assets.Scripts.Blocks.commands;
using Assets.Scripts.Blocks.components;
using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.Grid.components;
using Assets.Scripts.Player.Interfaces;
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
        private PlayerInputManager playerInputManager;

        private bool _isMovingExecutionFlag = false;


        private ITakeBlockCommand _target;

        private void Awake()
        {
            commandManager = new CommandManager();
        }

        private void Start()
        {
            blockManager = BlockManager.Instance;
            playerInputManager = PlayerInputManager.Instance;

            blockManager.OnTargetCreated += OnBlockCreated;

            playerInputManager.OnRotateRightPressed += RotateTargetEntity;
            playerInputManager.OnMovementPressed += MoveTargetEntity;
        }


        private void OnDestroy()
        {
            blockManager.OnTargetCreated -= OnBlockCreated;

            playerInputManager.OnRotateRightPressed -= RotateTargetEntity;
            playerInputManager.OnMovementPressed -= MoveTargetEntity;
        }

        private void RotateTargetEntity()
        {
            Debug.Log("Rotatin");
            RotateTargetEntity(1);
            return;
        }

        private async void MoveTargetEntity(Vector2 direction)
        {
            if (commandManager.IsExecuting)
                return;

            if (direction.y > 0)
                return;

            var gridDirection = new GridPosition((int)direction.x, (int)direction.y);
            var isValidMove = _target.CheckForValidMove(gridDirection);

            if(!isValidMove)
            {
                Debug.Log("Invalid move");
                if(gridDirection.y == -1)
                {
                    (_target as IGravity).SetEnable(false);
                }
                return;
            }
            var moveBlockCommandConfigurer = new MoveBlockCommandConfigurer(gridDirection);
            var command = new CommandManager.CommandBuilder().AddCommand<MoveBlockCommand>(_target, moveBlockCommandConfigurer).Build();
            await commandManager.ExecuteCommands(command, () => _isMovingExecutionFlag, (isMoving) => _isMovingExecutionFlag = isMoving);
        }

        private async void RotateTargetEntity(int direction)
        {
            if (commandManager.IsExecuting)
                return;

            var gridDirection = new GridPosition(-1 * direction, 1 * direction);
            var rotateBlockCommandConfigurer = new RotateBlockCommandConfigurer(gridDirection);
            var command = new CommandManager.CommandBuilder().AddCommand<RotateBlockCommand>(_target, rotateBlockCommandConfigurer).Build();
            await commandManager.ExecuteCommands(command);
        }

        private void OnBlockCreated(ITakeBlockCommand target)
        {
            SetTargetEntity(target);
        }

        private void SetTargetEntity(ITakeBlockCommand target)
        {

            if(_target is IPlayerControlled)
            {
                (_target as IPlayerControlled).SetEnabled(false);
            }

            _target = target;
            (_target as IPlayerControlled).SetEnabled(true);
            (_target as IGravity).SetEnable(true);

        }





    }
}