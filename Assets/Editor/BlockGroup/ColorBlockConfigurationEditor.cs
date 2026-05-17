using Assets.Editor.Components;
using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.Data;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


namespace Assets.Editor
{
    internal class ColorBlockConfigurationEditor : EditorWindow, IUseSaveAndLoadEditorComponent
    {

        //Static Members

        //Const Members
        private const int GRIDSIZE = 4;
        private GridPosition pivotPosition;


        private ColorBlockConfigurationData[,] grid = new ColorBlockConfigurationData[GRIDSIZE, GRIDSIZE];

        private string _colorBlockName;


        //EditorComponents
        private ColorBlockConfigSaveAndLoadComponent _saveLoadComponent;
        private ColorPaletteComponent _colorPaletteComponent;


        [MenuItem("Tools/Color Block Configuration Editor")]
        public static void Open()
        {
            GetWindow<ColorBlockConfigurationEditor>("Color Block Configuration Editor");
        }

        private void OnEnable()
        {
            _saveLoadComponent = new ColorBlockConfigSaveAndLoadComponent(this, GRIDSIZE);
            _saveLoadComponent.OnConfigurationSelected += LoadConfigurationIntoGrid;
            _saveLoadComponent.OnEnable();


            _colorPaletteComponent = new ColorPaletteComponent();
            _colorPaletteComponent.OnEnable();

        }

        private void OnDisable()
        {
            _saveLoadComponent.OnConfigurationSelected -= LoadConfigurationIntoGrid;
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();

            _saveLoadComponent.OnGUI();

            GUILayout.Space(10);

            GUILayout.BeginVertical();

            DrawGrid();
            _colorPaletteComponent.OnGUI();
            DrawOptions();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

        }

        private void DrawGrid()
        {
            for (int y = 0; y < GRIDSIZE; y++)
            {
                int flippedY = GRIDSIZE - 1 - y;
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < GRIDSIZE; x++)
                {
                    Color cellColor = grid[x, flippedY] != null ? ColorFromName(grid[x, flippedY].color.ToString()) : Color.black;

                    var oldColor = GUI.backgroundColor;

                    GUI.backgroundColor = cellColor;

                    if (GUILayout.Button("", GUILayout.Width(40), GUILayout.Height(40)))
                    {
                        if (grid[x, flippedY] == null)
                        {
                            grid[x, flippedY] = new ColorBlockConfigurationData { x = x, y = flippedY, color = _colorPaletteComponent.SelectedColor };
                        }
                        else if (grid[x, flippedY].color != _colorPaletteComponent.SelectedColor)
                        {
                            grid[x, flippedY].color = _colorPaletteComponent.SelectedColor;
                        }
                        else
                        {
                            grid[x, flippedY] = null;
                        }
                    }            
                    // Restore original color
                    GUI.backgroundColor = oldColor;

                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawOptions()
        {
            // Future options can be added here
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Name", GUILayout.Width(60));
            _colorBlockName = EditorGUILayout.TextField(_colorBlockName, GUILayout.Width(50));
            pivotPosition.x = Mathf.Clamp(pivotPosition.x, 0, 3);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            //Origin x/y options
            GUILayout.Label("Origin X", GUILayout.Width(60));
            pivotPosition.x = EditorGUILayout.IntField(pivotPosition.x, GUILayout.Width(50));
            pivotPosition.x = Mathf.Clamp(pivotPosition.x, 0, 3);

            GUILayout.Label("Y", GUILayout.Width(20));
            pivotPosition.y = EditorGUILayout.IntField(pivotPosition.y, GUILayout.Width(50));
            pivotPosition.y = Mathf.Clamp(pivotPosition.y, -3, 3);

            EditorGUILayout.EndHorizontal();  

        }

        private void LoadConfigurationIntoGrid(IDataConfiguration config)
        {
            _colorBlockName = config.name;
            pivotPosition = (config as ColorBlockGroupConfigurationData).pivotPosition;

            grid = new ColorBlockConfigurationData[GRIDSIZE, GRIDSIZE];

            foreach (var block in (config as ColorBlockGroupConfigurationData).blocks)
            {
                if (block.x < GRIDSIZE && block.y < GRIDSIZE)
                    grid[block.x, block.y] = new ColorBlockConfigurationData { x = block.x, y = block.y, color = block.color };
            }

        }

        void IUseSaveAndLoadEditorComponent.SaveCurrentConfiguration()
        {
            var blocks = new List<ColorBlockConfigurationData>();
            for (int y = 0; y < GRIDSIZE; y++)
            {
                for (int x = 0; x < GRIDSIZE; x++)
                {
                    if (grid[x, y] != null)
                    {
                        blocks.Add(grid[x, y]);
                    }
                }
            }

            _saveLoadComponent.UpdateConfiguration(blocks, _colorBlockName, pivotPosition);
            Repaint();
        }

        private Color ColorFromName(string name)
        {
            UnityEngine.ColorUtility.TryParseHtmlString(name, out var c);
            return c;
        }

    }
}
