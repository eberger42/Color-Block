using Assets.Editor.Data;
using Assets.Scripts.Blocks.components.colors;
using Assets.Scripts.Data;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.Components
{
    internal class PuzzleConfigSaveAndLoadComponent : SaveAndLoadEditorComponentaseBase
    {

        private readonly int _width;
        private readonly int _height;
        internal PuzzleConfigSaveAndLoadComponent(IUseSaveAndLoadEditorComponent listener, int width, int height) : base(listener)
        {
            _width = width;
            _height = height;
            _configurationCache = new ColorBlockGridConfigruationCache();
            _currentConfigurationGroup = new ColorBlockGridConfigurationData();
            _currentConfigurationGroup.id = GUID.Generate().ToString();
        }

        internal PuzzleConfigSaveAndLoadComponent(IUseSaveAndLoadEditorComponent listener) : base(listener)
        {
        }

        public override void OnEnable()
        {
            _configurationCache.LoadFromDisk();

            if ((_configurationCache as ColorBlockGridConfigruationCache).Configurations.Count > 0)
            {
                var firstConfig = (_configurationCache as ColorBlockGridConfigruationCache).Configurations[0];
                _currentConfigurationGroup = firstConfig;
                TriggerConfigurationSelected(firstConfig);
            }

            PuzzleConfigurationLoadButton.Refresh();
        }

        public override void Refresh()
        {
            PuzzleConfigurationLoadButton.Refresh();
        }

        public void UpdateConfiguration(List<ColorBLockGridNodeConfigurationData> configBlocks, List<ColorBlockConfigurationData> overlay, List<string> queue,  string name, int width, int height)
        {
            _currentConfigurationGroup.name = name;
            (_currentConfigurationGroup as ColorBlockGridConfigurationData).gridNodes = configBlocks;
            (_currentConfigurationGroup as ColorBlockGridConfigurationData).puzzleOverlay = overlay;
            (_currentConfigurationGroup as ColorBlockGridConfigurationData).queue = queue;
            (_currentConfigurationGroup as ColorBlockGridConfigurationData).width = width;
            (_currentConfigurationGroup as ColorBlockGridConfigurationData).height = height;
            _configurationCache.UpdateConfiguration(_currentConfigurationGroup);
            PuzzleConfigurationLoadButton.Refresh();
        }
        
        public override void CreateNewConfiguration()
        {
            var newConfig = new ColorBlockGridConfigurationData();
            (newConfig as IDataConfiguration).id = GUID.Generate().ToString();

            _currentConfigurationGroup = newConfig;

            TriggerConfigurationSelected(_currentConfigurationGroup);
        }

        protected override void DrawSavedList()
        {
            GUILayout.Label($"Saved Puzzle Levels", EditorStyles.boldLabel);
            GUILayout.Label($"Currently Editing: { _currentConfigurationGroup?.name }", EditorStyles.boldLabel);

            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(250), GUILayout.Height(400));

            foreach (var config in (_configurationCache as ColorBlockGridConfigruationCache).Configurations)
            {
                GUILayout.BeginHorizontal();

                var loadButton = new PuzzleConfigurationLoadButton(config, _width, _height);
                var pressed = loadButton.Draw(_currentConfigurationGroup);
                if (pressed)
                {
                    _currentConfigurationGroup = config;
                    TriggerConfigurationSelected(config);
                }

                GUILayout.EndHorizontal();

            }

            GUILayout.EndScrollView();
        }

        protected override void PostDataLoad()
        {
            _currentConfigurationGroup = (_configurationCache as ColorBlockGridConfigruationCache).Configurations.Count > 0 ? (_configurationCache as ColorBlockGridConfigruationCache).Configurations[0] : null;

            if (_currentConfigurationGroup != null)
                TriggerConfigurationSelected(_currentConfigurationGroup);
        }

        private class PuzzleConfigurationLoadButton
        {

            private readonly int _width;
            private readonly int _height;

            private static Dictionary<string, Texture2D> _previewCache = new Dictionary<string, Texture2D>();

            private Texture2D _preview;
            private readonly IDataConfiguration config;

            public PuzzleConfigurationLoadButton(IDataConfiguration config, int width, int _height)
            {
                this.config = config;
                _height = _height;
                _width = width;
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
