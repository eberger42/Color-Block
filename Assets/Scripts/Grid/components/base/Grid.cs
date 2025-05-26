using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.Blocks.scriptable_objects;
using Assets.Scripts.Grid.interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Grid.components
{
    public class Grid<T> : IGrid<T> where T : INode, new()
    {
        private int width;
        private int height;
        private float cellSize = 1f; // Assuming each cell is 1 unit in size
        private Vector2 offset;

        private T[,] gridArray;

        public Grid(int width, int height, float cellSize, Vector2 offset)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
            this.offset = offset;

            this.gridArray = new T[width, height];

        }

        public void GenerateGrid(NodeConfiguration config)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    T node = new T();
                    var origin = new Vector2(x * cellSize, y * cellSize) + offset;
                    node.Configure(config, new GridPosition(x,y));
                    Debug.Log($"Grid: {this}");
                    node.SetGridListener(this as IGrid<INode>);
                    node.GenerateNode();
                    gridArray[x, y] = node;
                }
            }
        }

        public T GetNode(int x, int y)
        {
            if (!IsInBounds(x, y))
                return default(T);

            return gridArray[x, y];
        }

        public bool IsInBounds(int x, int y)
        {
            var isInBounds = x >= 0 && x < width && y >= 0 && y < height;
            return isInBounds;
        }
        
        public bool IsSpacesOccupied(List<GridPosition> positions)
        {
            foreach (var position in positions)
            {
                if (!IsInBounds(position.x, position.y))
                {
                    Debug.LogError($"Position {position.x}, {position.y} is out of bounds.");
                    return true;
                }

                if (gridArray[position.x, position.y] != null)
                {
                    var node = gridArray[position.x, position.y];
                    if (node.IsOccupied())
                    {
                        return true;
                    }
                }
            }
            Debug.Log("All positions are free.");
            return false;
        }
        
        public void SetNodeData(int x, int y, INodeData nodeData)
        {
            if (!IsInBounds(x, y))
                return;
           
            var node = gridArray[x,y];

            if (node != null)
            {
                node.SetNodeData(nodeData);
            }

            
        }
    }
}