using Assets.Scripts.Blocks.components;
using Assets.Scripts.Blocks.scriptable_objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Scripts.Grid.components
{
    public class ColorGridManager : MonoBehaviour
    {

        public static ColorGridManager Instance { get; private set; } = null;

        private Grid<BlockNode> colorBlockGrid;

        [SerializeField, Range(1, 40)]
        private int gridWidth = 10;

        [SerializeField, Range(1, 40)]
        private int gridHeight = 10;

        [SerializeField]
        private Vector2 origin;

        private GridPosition placementPosition = new GridPosition(5, 0);


        [SerializeField]
        private NodeConfiguration config;



        private void Awake()
        {
            colorBlockGrid = new Grid<BlockNode>(gridWidth, gridHeight, 1, origin);

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
            colorBlockGrid.GenerateGrid(config);

            BlockManager.Instance.OnBlockCreated += PlaceBlock;
        }

        private void OnDestroy()
        {
            BlockManager.Instance.OnBlockCreated -= PlaceBlock;

        }

        public bool CheckIfSpacesAreOccupied(List<GridPosition> positions)
        {
            return colorBlockGrid.IsSpacesOccupied(positions);
        }

        public bool CheckIfSpacesAreOccupied(List<GridPosition> positions, GridPosition directionVector)
        {
            var newPositions = positions.ConvertAll(pos => pos + directionVector);

            Debug.Log($"Checking if spaces are available for positions: {string.Join(", ", newPositions)}");
            return CheckIfSpacesAreOccupied(newPositions);
        }




        private void PlaceBlock(ColorBlock block)
        {
            colorBlockGrid.SetNodeData<ColorBlock>(placementPosition.x, placementPosition.y, block);
        }


    }
}