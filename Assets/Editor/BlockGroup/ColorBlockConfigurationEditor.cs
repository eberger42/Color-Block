using Assets.Editor.Components;
using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.Data;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Assets.Editor
{
    internal class ColorBlockConfigurationEditor : EditorWindow, IUseSaveAndLoadEditorComponent
    {

        //Static Members
        private static readonly ColorType[] PaletteOrder = {
            ColorType.Red, ColorType.Blue, ColorType.Yellow,
            ColorType.Purple, ColorType.Orange, ColorType.Green
        };

        //Const Members
        private const int GRIDSIZE = 4;

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
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

        }

        private void DrawGrid()
        {
            for (int y = 0; y < GRIDSIZE; y++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < GRIDSIZE; x++)
                {
                    Color cellColor = grid[x, y] != null ? ColorFromName(grid[x, y].color.ToString()) : Color.white;

                    var oldColor = GUI.backgroundColor;

                    GUI.backgroundColor = cellColor;

                    if (GUILayout.Button("", GUILayout.Width(40), GUILayout.Height(40)))
                    {
                        if (grid[x, y] == null)
                        {
                            grid[x, y] = new ColorBlockConfigurationData { x = x, y = y, color = _colorPaletteComponent.SelectedColor };
                        }
                        else if (grid[x, y].color != _colorPaletteComponent.SelectedColor)
                        {
                            grid[x, y].color = _colorPaletteComponent.SelectedColor;
                        }
                        else
                        {
                            grid[x, y] = null;
                        }
                    }            
                    // Restore original color
                    GUI.backgroundColor = oldColor;

                }
                EditorGUILayout.EndHorizontal();
            }
        }


        private void LoadConfigurationIntoGrid(IDataConfiguration config)
        {
            _colorBlockName = config.name;

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

            _saveLoadComponent.UpdateConfiguration(blocks, _colorBlockName);
            Repaint();
        }

        private Color ColorFromName(string name)
        {
            ColorUtility.TryParseHtmlString(name, out var c);
            return c;
        }

    }
}
