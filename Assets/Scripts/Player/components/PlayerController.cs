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

        private bool _isMovingExecutionFlag = false;


        private ITakeBlockCommand _target;

        private void Awake()
        {
            commandManager = new CommandManager();
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

            if (Input.GetKeyDown(KeyCode.Space))
            {
                RotateTargetEntity(1);
                return;
            }
            if (commandManager.IsExecuting)
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