using Assets.Scripts.Blocks.components.colors;
using Assets.Scripts.Blocks.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using static Assets.Editor.ColorBlockConfigurationEditor;

namespace Assets.Editor.Components
{
    internal class ColorPaletteComponent : EditorComponentBase
    {
        
        //Static Members
        private static readonly ColorType[] PaletteOrder = {
            ColorType.Red, ColorType.Blue, ColorType.Yellow,
            ColorType.Purple, ColorType.Orange, ColorType.Green
        };


        private List<ColorPaletteButton> _paletteButtons = new List<ColorPaletteButton>();
        private ColorType selectedColor = ColorType.Red;

        //Properties
        internal ColorType SelectedColor => selectedColor;

        public override void OnEnable()
        {
            foreach (var colorType in PaletteOrder)
            {
                var color = BlockColor.ColorTypeToColorMap[colorType];
                _paletteButtons.Add(new ColorPaletteButton(colorType, color));
            }
        }

        public override void OnGUI()
        {
            DrawPalette();
        }

        private void DrawPalette()
        {
            GUILayout.Space(10);

            GUILayout.Label("Color Palette", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();

            foreach (var btn in _paletteButtons)
            {
                if (btn.Draw(selectedColor))
                    selectedColor = btn.ColorType;
            }

            GUILayout.EndHorizontal();
        }
    }
}
