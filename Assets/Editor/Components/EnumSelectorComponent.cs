using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.Components
{
    internal class EnumSelectorComponent<T> : EditorComponentBase
    {

        internal event Action<T> OnValueChanged;

        internal enum ViewMode
        {
            Dropdown,
            Buttons
        }

        private T _selectedValue; private int _selectedIndex = 0;

        private readonly string[] _enumNames;

        internal T SelectedValue { get => _selectedValue; }
        internal ViewMode CurrentViewMode { get; set; } = ViewMode.Buttons;

        public EnumSelectorComponent()
        {
            _selectedValue = default(T);

            _enumNames = Enum.GetNames(typeof(T));
        }

        public override void OnEnable()
        {
            throw new NotImplementedException();
        }

        public override void OnGUI()
        {
            switch(CurrentViewMode)
            {
                case ViewMode.Buttons:
                    DrawEnumSelectorButtons();
                    break;
                case ViewMode.Dropdown:
                    DrawEnumSelectorDropdown();
                    break;
            }
        }

        private void DrawEnumSelectorButtons()
        {
            GUILayout.BeginHorizontal();

            var enumValues = Enum.GetValues(typeof(T)).Cast<T>();

            foreach (var enumValue in enumValues)
            {
                if (GUILayout.Toggle(EqualityComparer<T>.Default.Equals(_selectedValue, enumValue), enumValue.ToString(), "Button"))
                    _selectedValue = enumValue;
            }
            GUILayout.EndHorizontal();
        }

        private void DrawEnumSelectorDropdown()
        {
            int selectedIndex = Array.IndexOf(Enum.GetValues(typeof(T)), _selectedValue);
            int newSelectedIndex = EditorGUILayout.Popup(selectedIndex, _enumNames);
            if (newSelectedIndex != selectedIndex)
                _selectedValue = (T)Enum.GetValues(typeof(T)).GetValue(newSelectedIndex);
        }
    }
}
