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
        private const int GridSize = 4;
        private ColorBlockConfigurationData[,] grid = new ColorBlockConfigurationData[GridSize, GridSize];

        private ColorBlockGroupConfigurationData _currentConfigurationGroup = new();

        private string _colorBlockName;

        private List<ColorPaletteButton> _paletteButtons = new List<ColorPaletteButton>();
        private ColorType selectedColor = ColorType.Red;

        private Vector2 scrollPos;

        private static readonly ColorType[] PaletteOrder = {
            ColorType.Red, ColorType.Blue, ColorType.Yellow,
            ColorType.Purple, ColorType.Orange, ColorType.Green
        };


        [MenuItem("Tools/Color Block Configuration Editor")]
        public static void Open()
        {
            GetWindow<ColorBlockConfigurationEditor>("Color Block Configuration Editor");
            ColorBlockConfigurationLoadButton.Refresh();
        }

        private void OnEnable()
        {
            ColorBlockConfigruationCache.LoadFromDisk();

            foreach (var colorType in PaletteOrder)
            {
                var color = BlockColor.ColorTypeToColorMap[colorType];
                _paletteButtons.Add(new ColorPaletteButton(colorType, color));
            }
            if (ColorBlockConfigruationCache.Configurations.Count > 0)
            {
                LoadConfigurationIntoGrid(ColorBlockConfigruationCache.Configurations[0]);
                _currentConfigurationGroup = ColorBlockConfigruationCache.Configurations[0];
            }
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.Width(250));
            GUILayout.BeginHorizontal();
            DrawSaveAndLoadToDiskButtons();
            GUILayout.EndHorizontal();
            DrawSavedList();
            GUILayout.EndVertical();

            GUILayout.Space(10);

            GUILayout.BeginVertical();

            DrawGrid();
            DrawPalette();
            DrawSaveLoadNew();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

        }

        private void DrawSaveAndLoadToDiskButtons()
        {
            if (GUILayout.Button("Save All To Disk"))
            {
                ColorBlockConfigruationCache.SaveToDisk();
            }
            if (GUILayout.Button("Load From Disk"))
            {
                ColorBlockConfigruationCache.LoadFromDisk();
                ColorBlockConfigurationLoadButton.Refresh();

                if(ColorBlockConfigruationCache.Configurations.Count > 0)
                {
                    LoadConfigurationIntoGrid(ColorBlockConfigruationCache.Configurations[0]);
                    _currentConfigurationGroup = ColorBlockConfigruationCache.Configurations[0];
                }

            }
        }

        private void DrawSavedList()
        {
            GUILayout.Label("Saved Color Block Groups", EditorStyles.boldLabel);
            GUILayout.Label($"Currently Editing: {_currentConfigurationGroup.name}", EditorStyles.boldLabel);

            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(250), GUILayout.Height(400));

            foreach (var config in ColorBlockConfigruationCache.Configurations)
            {
                GUILayout.BeginHorizontal();

                var loadButton = new ColorBlockConfigurationLoadButton(config);
                var pressed = loadButton.Draw(_currentConfigurationGroup);
                if (pressed)
                {
                    LoadConfigurationIntoGrid(config);
                    _currentConfigurationGroup = config;
                }

                GUILayout.EndHorizontal();

            }

            GUILayout.EndScrollView();
        }

        private void DrawGrid()
        {
            for (int y = 0; y < GridSize; y++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < GridSize; x++)
                {
                    Color cellColor = grid[x, y] != null ? ColorFromName(grid[x, y].color.ToString()) : Color.white;

                    var oldColor = GUI.backgroundColor;

                    GUI.backgroundColor = cellColor;

                    if (GUILayout.Button("", GUILayout.Width(40), GUILayout.Height(40)))
                    {
                        if (grid[x, y] == null || grid[x, y].color != selectedColor)
                        {
                            grid[x, y] = new ColorBlockConfigurationData { x = x, y = y, color = selectedColor };
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

        private void DrawPalette()
        {
            GUILayout.Space(10);

            GUILayout.Label("Color Palette");

            GUILayout.BeginHorizontal();

            foreach (var btn in _paletteButtons)
            {
                if (btn.Draw(selectedColor))
                    selectedColor = btn.ColorType;
            }

            GUILayout.EndHorizontal();
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
                _currentConfigurationGroup = new ColorBlockGroupConfigurationData { id = GUID.Generate().ToString(), blocks = new List<ColorBlockConfigurationData>() };
                LoadConfigurationIntoGrid(_currentConfigurationGroup);
            }
        }


        private void LoadConfigurationIntoGrid(ColorBlockGroupConfigurationData config)
        {
            _colorBlockName = config.name;

            grid = new ColorBlockConfigurationData[GridSize, GridSize];

            foreach (var block in config.blocks)
            {
                if (block.x < GridSize && block.y < GridSize)
                    grid[block.x, block.y] = new ColorBlockConfigurationData { x = block.x, y = block.y, color = block.color };
            }
        }

        private void SaveCurrentColorBlockConfiguration()
        {
            var blocks = new List<ColorBlockConfigurationData>();
            for (int y = 0; y < GridSize; y++)
            {
                for (int x = 0; x < GridSize; x++)
                {
                    if (grid[x, y] != null)
                    {
                        blocks.Add(grid[x, y]);
                    }
                }
            }

            _currentConfigurationGroup.blocks = blocks;
            _currentConfigurationGroup.name = _colorBlockName;

            ColorBlockConfigruationCache.SaveConfiguration(_currentConfigurationGroup);

            ColorBlockConfigurationLoadButton.Refresh();
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

        public class ColorBlockConfigurationLoadButton
        {

            private static Dictionary<string, Texture2D> _previewCache = new Dictionary<string, Texture2D>();

            private Texture2D _preview;
            private readonly ColorBlockGroupConfigurationData config;

            public ColorBlockConfigurationLoadButton(ColorBlockGroupConfigurationData config)
            {
                this.config = config;

                if(_previewCache.ContainsKey(config.id))
                    _preview = _previewCache[config.id];
                else
                    SetPreview(config);
            }

            private Texture2D SetPreview(ColorBlockGroupConfigurationData config)
            {

                const int size = 64; // preview size
                const int cell = size / GridSize;

                _preview = new Texture2D(size, size);
                var clear = new Color(0, 0, 0, 0);

                // Fill transparent
                for (int y = 0; y < size; y++)
                    for (int x = 0; x < size; x++)
                        _preview.SetPixel(x, y, clear);

                // Draw blocks
                foreach (var block in config.blocks)
                {
                    var c = BlockColor.ColorTypeToColorMap[block.color];

                    for (int py = 0; py < cell; py++)
                    {
                        for (int px = 0; px < cell; px++)
                        {
                            int tx = block.x * cell + px;
                            int ty = (GridSize - 1 - block.y) * cell + py; // flip Y for UI
                            _preview.SetPixel(tx, ty, c);
                        }
                    }
                }

                _preview.Apply();

                _previewCache[config.id] = _preview;
                return _preview;
            }

            public bool Draw(ColorBlockGroupConfigurationData selected, int xOffset = 15, int yOffset = 10)
            {

                var rect = GUILayoutUtility.GetRect(64 + xOffset, 64 + yOffset, GUILayout.Width(64 + xOffset), GUILayout.Height(64 + yOffset));

                //Setting rect to be the size of the button with offset
                rect = new Rect(rect.x + xOffset * .5f, rect.y + yOffset * .5f, 64, 64);

                if (selected == config)
                {
                    var outline = new Rect(rect.x - 2, rect.y - 2, rect.width + 4, rect.height + 4);
                    EditorUtility.DrawOutline(outline, Color.white, 2);
                }

                GUI.DrawTexture(rect, _preview, ScaleMode.StretchToFill);

                GUILayout.BeginVertical();
                bool pressed = GUILayout.Button(config.name, GUILayout.Height(64));
                GUILayout.EndVertical();


                return pressed;
            }

            public static void Refresh()
            {
                _previewCache.Clear();
            }
        }
    }
}
