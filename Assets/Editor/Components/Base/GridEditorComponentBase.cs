
using System;
using System.Collections.Generic;
using UnityEditor;

namespace Assets.Editor.Components
{
    internal abstract class GridEditorComponentBase<T> : EditorComponentBase
    {

        protected int _width = 0;
        protected int _height = 0;

        protected T[,] _grid;


        protected GridEditorComponentBase(int width, int height) 
        {
            _width = width;
            _height = height;
        }


        public override void OnGUI()
        {

            if(IsActive)
                DrawGrid();
        }

        public override void OnEnable()
        {
            _grid = new T[_width, _height];
        }

        public void UpdateGridSize(int width, int height)
        {
            if (width == _width && height == _height)
                return;
            _width = width;
            _height = height;
            
            var newGrid = new T[_width, _height];

            for (int y = 0; y < Math.Min(_height, _grid.GetLength(1)); y++)
            {
                for (int x = 0; x < Math.Min(_width, _grid.GetLength(0)); x++)
                {
                    newGrid[x, y] = _grid[x, y];
                }
            }
            _grid = newGrid;
        }
        
        public List<T> GetGridConfiguration()
        {
            var config = new List<T>();
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    if (_grid[x, y] != null)
                    {
                        config.Add(_grid[x, y]);
                    }
                }
            }
            return config;
        }

        public virtual void LoadConfigurationIntoGrid(int width, int height, List<T> config)
        {
            _width = width;
            _height = height;
            _grid = new T[width, height];
        }

        protected void DrawGrid()
        {
            for (int y = 0; y < _height; y++)
            {
                EditorGUILayout.BeginHorizontal();

                for (int x = 0; x < _width; x++)
                {
                    UseDrawGrid(x, y);
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        protected abstract void UseDrawGrid(int x, int y);

        

    }
}
