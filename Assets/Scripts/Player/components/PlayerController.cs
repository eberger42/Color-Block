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

        private IEntity _targetEntity;

        private void Awake()
        {
            commandManager = new CommandManager();
            colorGridManager = FindFirstObjectByType<ColorGridManager>();
        }

        private void Start()
        {
            blockManager = BlockManager.Instance;
            Debug.Log(blockManager);
            blockManager.OnBlockCreated += OnBlockCreated;
        }

        private void OnDestroy()
        {

            blockManager.OnBlockCreated -= OnBlockCreated;
        }

        private void Update()
        {

            if(commandManager.IsExecuting)
                return;

            float vertical = Mathf.Round(Input.GetAxisRaw("Vertical"));
            float horizontal = Mathf.Round(Input.GetAxisRaw("Horizontal"));

            if (vertical != 0)
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
            var gridPositions = _targetEntity.GetGridPositions();

            var positionsToCheck = gridPositions.Where(pos => !gridPositions.Contains(pos + gridDirection)).ToList();
            var areSpacesOccupied = colorGridManager.CheckIfSpacesAreOccupied(positionsToCheck, gridDirection);

            if(areSpacesOccupied)
            {
                Debug.Log("Invalid move");
                return;
            }

            var moveBlockCommandConfigurer = new MoveBlockCommandConfigurer(gridDirection);
            var command = new CommandManager.CommandBuilder().AddCommand<MoveBlockCommand>(_targetEntity, moveBlockCommandConfigurer).Build();
            await commandManager.ExecuteCommands(command);
        }


        private void OnBlockCreated(ColorBlock block)
        {
            SetTargetEntity(block);
        }

        private void SetTargetEntity(IEntity targetEntity)
        {
            _targetEntity = targetEntity;
        }





    }
}