using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.Blocks.scriptable_objects;
using Assets.Scripts.General.interfaces;
using Assets.Scripts.Grid.interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Grid.components
{
    public class Grid<T> : IGrid<T> where T : INode, new()
    {

        public event Action<INodeEvent> OnNodeEvent;

        private int width;
        private int height;
        private float cellSize = 1f; // Assuming each cell is 1 unit in size
        private Vector2 offset;

        private ITickManager tickManager;
        private T[,] gridArray;
        private bool disposedValue;
    
        private IGridTickStrategy<T> tickStrategy = new DefaultTickStrategy();


        public Grid(int width, int height, float cellSize, Vector2 offset)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
            this.offset = offset;

            this.gridArray = new T[width, height];


        }

        public Grid(int width, int height, float cellSize, Vector2 offset, IGridTickStrategy<T> gridTickStrategy) : this(width, height, cellSize, offset)
        {
           this.tickStrategy = gridTickStrategy;
        }

        public void GenerateGrid(NodeConfiguration config, ITickManager tickManager)
        {
            this.tickManager = tickManager;
            tickManager.OnTick += (this as ITick).Tick;

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
                    node.OnNodeEvent += TriggerNodeEvent;

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



        ///////////////////////////////////////////////////////////
        /// ITick Interface
        ///////////////////////////////////////////////////////////
        void ITick.Tick()
        {
            this.tickStrategy.Tick(gridArray);
        }



        private void TriggerNodeEvent(INodeEvent nodeEvent)
        {
            OnNodeEvent?.Invoke(nodeEvent);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (gridArray != null)
                    {
                           foreach (var node in gridArray)
                        {
                            if (node != null)
                            {
                                node.OnNodeEvent -= TriggerNodeEvent;
                                tickManager.OnTick -= node.Tick;
                            }
                        }
                        gridArray = null;
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' h
        // managed resources
        // ~Grid()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public interface IGridTickStrategy<K>
        {

            public void Tick(K[,] gridArray);
        }

        private class DefaultTickStrategy : IGridTickStrategy<T>
        {
            void IGridTickStrategy<T>.Tick(T[,] gridArray)
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