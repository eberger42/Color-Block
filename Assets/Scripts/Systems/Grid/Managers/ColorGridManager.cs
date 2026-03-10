using Assets.Scripts.Blocks.commands;
using Assets.Scripts.Blocks.components;
using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.Blocks.scriptable_objects;
using Assets.Scripts.General;
using Assets.Scripts.General.interfaces;
using Assets.Scripts.Grid.interfaces;
using Assets.Scripts.Systems.LevelSelect;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Scripts.Grid.components
{
    public class ColorGridManager : MonoBehaviour
    {

        public static ColorGridManager Instance { get; private set; } = null;

        public event Action<INodeEvent> OnNodeEvent;

        public Grid<BlockNode> ColorBlockGrid { get => colorBlockGrid; }

        private Grid<BlockNode> colorBlockGrid;

        [SerializeField]
        private NodeConfiguration config;

        private readonly CommandManager commandManager = new CommandManager();


        //Properties
        public int GridWidth { get => colorBlockGrid.Width; }
        public int GridHeight { get => colorBlockGrid.Height; }
        private Vector2 Offset => colorBlockGrid.Offset;


        private void Awake()
        {

            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }

        }

        private void Start()
        {
            BlockManager.Instance.OnTargetCreated += PlaceBlock;

        }

        private void OnDestroy()
        {

            if(BlockManager.Instance != null)
                BlockManager.Instance.OnTargetCreated -= PlaceBlock;

            if(colorBlockGrid != null)
                colorBlockGrid.OnNodeEvent -= TriggerNodeEvent;

            Node.Dispose();

        }

        public void InitializeGrid(PuzzleGridConfiguration puzzleGridConfiguration)
        {
            var width = puzzleGridConfiguration.Width;
            var height = puzzleGridConfiguration.Height;
            GenerateGrid(width, height);
        }

        private void GenerateGrid(int width, int height)
        {
            float widthOffset, heightOffset;
            FindOriginOffset(width, height, out widthOffset, out heightOffset);

            var offset = new Vector2(-widthOffset, -heightOffset);
            config.Origin = offset;

            colorBlockGrid = new Grid<BlockNode>(width, height, 1, offset);
            colorBlockGrid.OnNodeEvent += TriggerNodeEvent;


            colorBlockGrid.GenerateGrid(config, (GameTickManager.Instance as ITickManager));
        }


        public Vector2 GetWorldPosition(GridPosition gridPosition)
        {
            return new Vector2(gridPosition.x, gridPosition.y) + Offset;
        }

        public bool CheckIfSpacesAreOccupied(List<GridPosition> positions)
        {
            return colorBlockGrid.IsSpacesOccupied(positions);
        }

        public bool CheckIfSpacesAreOccupied(List<GridPosition> positions, GridPosition directionVector)
        {
            var newPositions = positions.ConvertAll(pos => pos + directionVector);

            return CheckIfSpacesAreOccupied(newPositions);
        }

        private async void PlaceBlock(ITakeBlockCommand target)
        {
            Debug.Log($"ColorGrid BlockPlaced");

            var placeBlockCommandConfigurer = new PlaceBlockCommandConfigurer(colorBlockGrid, new GridPosition(GridWidth/2, GridHeight - 5));
            var command = new CommandManager.CommandBuilder().AddCommand<PlaceBlockCommand>(target, placeBlockCommandConfigurer).Build();
            await commandManager.ExecuteCommands(command);
        }

        private void TriggerNodeEvent(INodeEvent nodeEvent)
        {
            OnNodeEvent?.Invoke(nodeEvent);
        }

        private class ColorGridTickStrategy : Grid<BlockNode>.IGridTickStrategy<BlockNode>
        {
            void Grid<BlockNode>.IGridTickStrategy<BlockNode>.Tick(BlockNode[,] gridArray)
            {
                var width = gridArray.GetLength(0);
                var height = gridArray.GetLength(1);

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        var node = gridArray[x, y];
                        if (node != null)
                        {
                            node.Tick();
                        }
                    }
                }
            }

        }


        private void FindOriginOffset(int width, int height, out float widthOffset, out float heightOffset)
        {
            bool isWidthEven = width % 2 == 0;
            bool isHeightEven = height % 2 == 0;

            widthOffset = isWidthEven ? (width / 2f) - 0.5f : Mathf.Floor(width / 2f);
            heightOffset = isHeightEven ? (height / 2f) - 0.5f : Mathf.Floor(height / 2f);

        }

    }
}