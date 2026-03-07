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
