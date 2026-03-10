using Assets.Editor.Components.Base;
using Assets.Scripts.Blocks.components.colors;
using Assets.Scripts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.Puzzle
{
    internal class ColorBlockQueueComponent : QueueEditorComponentBase<string>
    {
        private const int GRIDSIZE = 4;
        private static Dictionary<string, Texture2D> _previewCache = new Dictionary<string, Texture2D>();
        private readonly ColorBlockConfigurationCache _colorBlockConfigurationCache = new();

        public ColorBlockQueueComponent()
        {
            (_colorBlockConfigurationCache as IDataConfigurationCache).LoadFromDisk();
            var _blockData = _colorBlockConfigurationCache.Configurations;

            foreach (var config in _blockData)
            {
                if (!_previewCache.ContainsKey(config.id))
                {
                    AddPreview(config);
                }
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();

            _reorderableList.onAddCallback = list =>
            {
                // Always safe — does NOT depend on layout
                var mousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);

                PopupWindow.Show(
                    new Rect(mousePos.x, mousePos.y, 0, 0),
                    new TexturePickerPopup(_previewCache, (key, tex) =>
                    {
                        _queue.Add((string)(object)key);

                    })
                );
            };

        }
        public void LoadConfigurationIntoQueue(List<string> configIds)
        {
            _queue.Clear();
            foreach (var id in configIds)
            {
                if (_colorBlockConfigurationCache.Configurations.Any(c => c.id == id))
                {
                    _queue.Add(id);
                }
            }
        }

        protected override void UseDrawQueueItem(string item, Rect rect)
        {

            var tex = _previewCache[item];
            GUI.DrawTexture(rect, tex, ScaleMode.ScaleToFit);

        }

        private Texture2D AddPreview(IDataConfiguration config)
        {

            int size = 64; // preview size
            int cell = size / GRIDSIZE;

            var preview = new Texture2D(size, size);
            var clear = new Color(0, 0, 0, 0);

            // Fill transparent
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                    preview.SetPixel(x, y, clear);

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
                        preview.SetPixel(tx, ty, c);
                    }
                }
            }

            preview.Apply();

            _previewCache[config.id] = preview;
            return preview;
        }

    }

    class TexturePickerPopup : PopupWindowContent
    {
        private readonly Dictionary<string, Texture2D> _cache;
        private readonly Action<string, Texture2D> _onSelected;

        public TexturePickerPopup(
            Dictionary<string, Texture2D> cache,
            Action<string, Texture2D> onSelected)
        {
            _cache = cache;
            _onSelected = onSelected;
        }

        public override Vector2 GetWindowSize() => new Vector2(250, 300);

        public override void OnGUI(Rect rect)
        {
            foreach (var kvp in _cache)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label(kvp.Value, GUILayout.Width(40), GUILayout.Height(40));

                if (GUILayout.Button(kvp.Key, GUILayout.Height(40)))
                {
                    _onSelected(kvp.Key, kvp.Value);
                    editorWindow.Close();
                }

                GUILayout.EndHorizontal();
            }
        }
    }

}
