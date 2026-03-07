using Assets.Scripts.Blocks.components.colors;
using Assets.Scripts.Blocks.interfaces;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Assets.Editor.ColorBlockConfigurationEditor;

namespace Assets.Editor.Components
{
    internal class EntityPaletteComponent : EditorComponentBase
    {
        

        private List<EntityPaletteButton> _paletteButtons = new List<EntityPaletteButton>();
        private EntityType _selectedEntity = EntityType.RedBlock;

        //Properties
        internal EntityType SelectedEntity => _selectedEntity;

        public override void OnEnable()
        {
            foreach(EntityType entityType in System.Enum.GetValues(typeof(EntityType)))
            {
                _paletteButtons.Add(new EntityPaletteButton(entityType));
            }
        }

        public override void OnGUI()
        {
            DrawPalette();
        }

        private void DrawPalette()
        {
            GUILayout.Space(10);

            GUILayout.Label("Entity Palette", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();

            foreach (var btn in _paletteButtons)
            {
                if (btn.Draw(_selectedEntity))
                    _selectedEntity = btn.EntityType;
            }

            GUILayout.EndHorizontal();
        }

        //////////////////////////////////////////////////////////////////
        /// Helper Classes
        //////////////////////////////////////////////////////////////////
        public class EntityPaletteButton
        {

            public static readonly Dictionary<EntityType, Texture2D> EntityIcons = new();
            public EntityType EntityType { get; }
            private Texture2D icon;

            private bool _isSelected = false;

            public EntityPaletteButton(EntityType entityType)
            {
                EntityType = entityType;

                if(EntityIcons.ContainsKey(entityType))
                {
                    icon = EntityIcons[entityType];
                }
                else
                {
                    string iconPath = $"Assets/Editor/Icons/{entityType.ToString().ToLower()}_icon.png";
                    icon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
                    EntityIcons[entityType] = icon;
                }

            }

            public bool Draw(EntityType selected, int size = 30, float xOffset = 3, float yOffset = 0)
            {

                //Reserve space for button with offset
                var rect = GUILayoutUtility.GetRect(size + xOffset, size + yOffset, GUILayout.Width(size + xOffset), GUILayout.Height(size + yOffset));

                //Setting rect to be the size of the button with offset
                rect = new Rect(rect.x + xOffset * .5f, rect.y + yOffset * .5f, size, size);

                if (selected == EntityType)
                {
                    var outline = new Rect(rect.x - 2, rect.y - 2, rect.width + 4, rect.height + 4);
                    EditorGUI.DrawRect(outline, Color.white);
                }

                if(icon != null)
                {
                    GUI.DrawTexture(rect, icon, ScaleMode.ScaleToFit);
                }else
                {
                    EditorGUI.DrawRect(rect, Color.magenta);
                }
                return GUI.Button(rect, GUIContent.none, GUIStyle.none);
            }


        }

    }
}
