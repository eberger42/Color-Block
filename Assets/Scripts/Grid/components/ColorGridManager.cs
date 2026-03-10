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

        [SerializeField, Range(1, 40)]
        private int gridWidth = 10;

        [SerializeField, Range(1, 40)]
        private int gridHeight = 10;

        [SerializeField]
        private Vector2 origin;



        [SerializeField]
        private NodeConfiguration config;

        private readonly CommandManager commandManager = new CommandManager();
        private GridPosition placementPosition = new GridPosition(20, 18);

        private SceneController sceneController;


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

            sceneController = SceneController.instance;

            sceneController.OnSceneLoaded += SceneController_OnSceneLoaded;

        }

        private void SceneController_OnSceneLoaded(Scenes scene)
        {
            var puzzleGridConfig = LevelSelectManager.Instance.GridConfiguration;
            if (puzzleGridConfig != null)
            {
                InitializeGrid(puzzleGridConfig);
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

            gridWidth = puzzleGridConfiguration.Width;
            gridHeight = puzzleGridConfiguration.Height;

            colorBlockGrid = new Grid<BlockNode>(gridWidth, gridHeight, 1, origin);
            colorBlockGrid.OnNodeEvent += TriggerNodeEvent;


            colorBlockGrid.GenerateGrid(config, (GameTickManager.Instance as ITickManager));
        }

        public Vector2 GetWorldPosition(GridPosition gridPosition)
        {
            return new Vector2(gridPosition.x, gridPosition.y);
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
            var placeBlockCommandConfigurer = new PlaceBlockCommandConfigurer(colorBlockGrid, placementPosition);
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

    }
}