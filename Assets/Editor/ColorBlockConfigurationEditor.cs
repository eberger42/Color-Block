using Assets.Editor.Components;
using Assets.Editor.Data;
using Assets.Scripts.Blocks.components.colors;
using Assets.Scripts.Blocks.interfaces;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Assets.Editor
{
    internal class ColorBlockConfigurationEditor : EditorWindow
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

        private List<ColorPaletteButton> _paletteButtons = new List<ColorPaletteButton>();


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
            _saveLoadComponent = new ColorBlockConfigSaveAndLoadComponent(GRIDSIZE);
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
            DrawSaveLoadNew();
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

        private void DrawSaveLoadNew()
        {
            GUILayout.Space(10);
            _colorBlockName = EditorGUILayout.TextField("Color Block Name", _colorBlockName);

            if (GUILayout.Button("Save Configuration"))
            {
                SaveCurrentColorBlockConfiguration();
            }
            if (GUILayout.Button("New Configuration"))
            {
                _saveLoadComponent.CreateNewConfiguration();
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

        private void SaveCurrentColorBlockConfiguration()
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



        //////////////////////////////////////////////////////////////////
        /// Helper Classes
        //////////////////////////////////////////////////////////////////
        public class ColorPaletteButton
        {
            public ColorType ColorType { get; }
            public Color Color { get; }

            private bool _isSelected = false;

            public ColorPaletteButton(ColorType colorType, Color color)
            {
                ColorType = colorType;
                Color = color;
            }

            public bool Draw(ColorType selected, int size = 30, float xOffset = 3, float yOffset = 0)
            {

                //Reserve space for button with offset
                var rect = GUILayoutUtility.GetRect(size + xOffset, size + yOffset, GUILayout.Width(size + xOffset), GUILayout.Height(size + yOffset));

                //Setting rect to be the size of the button with offset
                rect = new Rect(rect.x + xOffset * .5f, rect.y + yOffset * .5f, size, size);

                if (selected == ColorType)
                {
                    var outline = new Rect(rect.x - 2, rect.y - 2, rect.width + 4, rect.height + 4);
                    EditorGUI.DrawRect(outline, Color.white);
                }

                EditorGUI.DrawRect(rect, Color);
                return GUI.Button(rect, GUIContent.none, GUIStyle.none);
            }


        }

    }
}
