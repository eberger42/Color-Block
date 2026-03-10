
using Assets.Scripts.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Editor.Components
{
    internal class EntityGridComponent : GridEditorComponentBase<GridNodeConfigurationData>
    {
        //Const
        private const int CELL_WIDTH = 30;
        private const int CELL_HEIGHT = 30;


        private EntityPaletteComponent _paletteComponent;

        public EntityGridComponent(int width, int height) : base(width, height)
        {
        }

        public override void LoadConfigurationIntoGrid(int width, int height, List<GridNodeConfigurationData> config)
        {
            base.LoadConfigurationIntoGrid(width, height, config);

            foreach (var block in config)
            {
                if (block.x < _width && block.y < _height)
                    _grid[block.x, block.y] = new GridNodeConfigurationData { x = block.x, y = block.y };
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();

            _paletteComponent = new EntityPaletteComponent();
            _paletteComponent.OnEnable();
        }

        public override void OnGUI()
        {
            base.OnGUI();
            _paletteComponent.OnGUI();
        }

        protected override void UseDrawGrid(int x, int y)
        {
            Texture2D cellTexture =
                       _grid[x, y] != null
                       ? EntityPaletteComponent.EntityPaletteButton.EntityIcons[_grid[x, y].entityType]
                       : null;

            // Draw a real button to get correct spacing + grey background
            if (GUILayout.Button(GUIContent.none, GUILayout.Width(CELL_WIDTH), GUILayout.Height(CELL_HEIGHT)))
            {
                if (_grid[x, y] == null)
                {
                    _grid[x, y] = new GridNodeConfigurationData
                    {
                        x = x,
                        y = y,
                        entityType = _paletteComponent.SelectedEntity
                    };
                }
                else if (_grid[x, y].entityType != _paletteComponent.SelectedEntity)
                {
                    _grid[x, y].entityType = _paletteComponent.SelectedEntity;
                }
                else
                {
                    _grid[x, y] = null;
                }
            }

            // Get the rect of the button we just drew
            Rect rect = GUILayoutUtility.GetLastRect();

            // Draw the texture on top
            if (cellTexture != null)
                GUI.DrawTexture(rect, cellTexture, ScaleMode.ScaleToFit);
        }

        private Color ColorFromName(string name)
        {
            ColorUtility.TryParseHtmlString(name, out var c);
            return c;
        }
    }
}
