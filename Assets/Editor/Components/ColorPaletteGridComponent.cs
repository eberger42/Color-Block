using Assets.Editor.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Editor.Components
{
    internal class ColorPaletteGridComponent : GridEditorComponentBase<ColorBlockConfigurationData>
    {
        //Const
        private const int CELL_WIDTH = 30;
        private const int CELL_HEIGHT = 30;


        private ColorPaletteComponent _colorPaletteComponent;

        public ColorPaletteGridComponent(int width, int height) : base(width, height)
        {
        }

        public override void LoadConfigurationIntoGrid(int width, int height,  List<ColorBlockConfigurationData> config)
        {
            base.LoadConfigurationIntoGrid(width, height, config);

            foreach (var block in config)
            {
                if (block.x < _width && block.y < (_height))
                    _grid[block.x, block.y] = new ColorBlockConfigurationData { x = block.x, y = block.y, color = block.color };
            }

        }

        public override void OnEnable()
        {
            base.OnEnable();

            _colorPaletteComponent = new ColorPaletteComponent();
            _colorPaletteComponent.OnEnable();
        }

        public override void OnGUI()
        {
            base.OnGUI();
            _colorPaletteComponent.OnGUI();
        }

        protected override void UseDrawGrid(int x, int y)
        {
            Color cellColor = _grid[x, y] != null ? ColorFromName(_grid[x, y].color.ToString()) : Color.white;

            var oldColor = GUI.backgroundColor;

            GUI.backgroundColor = cellColor;

            if (GUILayout.Button("", GUILayout.Width(CELL_WIDTH), GUILayout.Height(CELL_HEIGHT)))
            {
                if (_grid[x, y] == null)
                {
                    _grid[x, y] = new ColorBlockConfigurationData { x = x, y = y, color = _colorPaletteComponent.SelectedColor };
                }
                else if (_grid[x, y].color != _colorPaletteComponent.SelectedColor)
                {
                    _grid[x, y].color = _colorPaletteComponent.SelectedColor;
                }
                else
                {
                    _grid[x, y] = null;
                }
            }
            // Restore original color
            GUI.backgroundColor = oldColor;
        }

        private Color ColorFromName(string name)
        {
            ColorUtility.TryParseHtmlString(name, out var c);
            return c;
        }
    }
}
