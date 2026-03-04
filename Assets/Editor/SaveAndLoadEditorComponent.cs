

using Assets.Scripts.Blocks.components.colors;
using Assets.Scripts.Data;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Assets.Editor.ColorBlockConfigurationEditor;

namespace Assets.Editor
{
    internal class SaveAndLoadEditorComponent
    {

        public event Action OnSaveAllPressed;
        public event Action OnLoadPressed;

        private readonly string TARGETNAME = "";

        public Vector2 scrollPos;


        public SaveAndLoadEditorComponent(string targetName)
        {
            TARGETNAME = targetName;
        }

        public void OnGUI()
        {
            GUILayout.BeginVertical(GUILayout.Width(250));
            GUILayout.BeginHorizontal();
            DrawSaveAndLoadToDiskButtons();
            GUILayout.EndHorizontal();
            DrawSavedList();
            GUILayout.EndVertical();
        }

        ///////////////////////////////////////////////////////////////////
        /// Private Methods
        ///////////////////////////////////////////////////////////////////
        private void DrawSaveAndLoadToDiskButtons()
        {
            if (GUILayout.Button("Save All To Disk"))
            {
                OnSaveAllPressed?.Invoke();
                ColorBlockConfigruationCache.SaveToDisk();
            }
            if (GUILayout.Button("Load From Disk"))
            {
                OnLoadPressed?.Invoke();
            }
        }

        private void DrawSavedList()
        {
            GUILayout.Label($"Saved {TARGETNAME}", EditorStyles.boldLabel);
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



        private class ColorBlockConfigurationLoadButton
        {

            private static Dictionary<string, Texture2D> _previewCache = new Dictionary<string, Texture2D>();

            private Texture2D _preview;
            private readonly ColorBlockGroupConfigurationData config;

            public ColorBlockConfigurationLoadButton(ColorBlockGroupConfigurationData config)
            {
                this.config = config;

                if (_previewCache.ContainsKey(config.id))
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
