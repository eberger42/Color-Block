using Assets.Editor.Data;
using Assets.Scripts.Blocks.components.colors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using static Assets.Editor.ColorBlockConfigurationEditor;

namespace Assets.Editor
{
    public class ColorBlockConfigSaveAndLoadComponent : SaveAndLoadEditorComponent
    {

        private readonly int GRIDSIZE = 4;
        public ColorBlockConfigSaveAndLoadComponent(int gridsize)
        {
            GRIDSIZE = gridsize;
            _configurationCache = new ColorBlockConfigurationCache();
            _currentConfigurationGroup = new ColorBlockGroupConfigurationData();
            _currentConfigurationGroup.id = GUID.Generate().ToString();
        }

        public override void OnEnable()
        {
            _configurationCache.LoadFromDisk();

            if ((_configurationCache as ColorBlockConfigurationCache).Configurations.Count > 0)
            {
                var firstConfig = (_configurationCache as ColorBlockConfigurationCache).Configurations[0];
                _currentConfigurationGroup = firstConfig;
                TriggerConfigurationSelected(firstConfig);
            }

            ColorBlockConfigurationLoadButton.Refresh();
        }

        public override void Refresh()
        {
            ColorBlockConfigurationLoadButton.Refresh();
        }

        public void UpdateConfiguration(List<ColorBlockConfigurationData> configBlocks, string name)
        {
            (_currentConfigurationGroup as ColorBlockGroupConfigurationData).blocks = configBlocks;
            _currentConfigurationGroup.name = name;
            _configurationCache.UpdateConfiguration(_currentConfigurationGroup);
            ColorBlockConfigurationLoadButton.Refresh();
        }
        
        public void CreateNewConfiguration()
        {
            var newConfig = new ColorBlockGroupConfigurationData { blocks = new List<ColorBlockConfigurationData>() };
            (newConfig as IDataConfiguration).id = GUID.Generate().ToString();

            _currentConfigurationGroup = newConfig;

            TriggerConfigurationSelected(_currentConfigurationGroup);
        }

        protected override void DrawSavedList()
        {
            GUILayout.Label($"Saved Color Block Groups", EditorStyles.boldLabel);
            GUILayout.Label($"Currently Editing: {(_currentConfigurationGroup as IDataConfiguration)?.name}", EditorStyles.boldLabel);

            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(250), GUILayout.Height(400));

            foreach (var config in (_configurationCache as ColorBlockConfigurationCache).Configurations)
            {
                Debug.Log($"Drawing config: {(config as IDataConfiguration).name}");
                GUILayout.BeginHorizontal();

                var loadButton = new ColorBlockConfigurationLoadButton(config, GRIDSIZE);
                var pressed = loadButton.Draw(_currentConfigurationGroup);
                if (pressed)
                {
                    TriggerConfigurationSelected(config);
                    _currentConfigurationGroup = config;
                }

                GUILayout.EndHorizontal();

            }

            GUILayout.EndScrollView();
        }


        private class ColorBlockConfigurationLoadButton
        {

            private readonly int GRIDSIZE;

            private static Dictionary<string, Texture2D> _previewCache = new Dictionary<string, Texture2D>();

            private Texture2D _preview;
            private readonly IDataConfiguration config;

            public ColorBlockConfigurationLoadButton(IDataConfiguration config, int gridSize)
            {
                this.config = config;
                GRIDSIZE = gridSize;

                if (_previewCache.ContainsKey(config.id))
                    _preview = _previewCache[config.id];
                else
                    SetPreview(config);
            }

            private Texture2D SetPreview(IDataConfiguration config)
            {

                int size = 64; // preview size
                int cell = size / GRIDSIZE;

                _preview = new Texture2D(size, size);
                var clear = new Color(0, 0, 0, 0);

                // Fill transparent
                for (int y = 0; y < size; y++)
                    for (int x = 0; x < size; x++)
                        _preview.SetPixel(x, y, clear);

                // Draw blocks
                foreach (var block in (config as ColorBlockGroupConfigurationData).blocks)
                {
                    var c = BlockColor.ColorTypeToColorMap[block.color];

                    for (int py = 0; py < cell; py++)
                    {
                        for (int px = 0; px < cell; px++)
                        {
                            int tx = block.x * cell + px;
                            int ty = (GRIDSIZE - 1 - block.y) * cell + py; // flip Y for UI
                            _preview.SetPixel(tx, ty, c);
                        }
                    }
                }

                _preview.Apply();

                _previewCache[config.id] = _preview;
                return _preview;
            }

            public bool Draw(IDataConfiguration selected, int xOffset = 15, int yOffset = 10)
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
                Debug.Log("Refreshing previews...");
                _previewCache.Clear();
            }
        }

    }
}
