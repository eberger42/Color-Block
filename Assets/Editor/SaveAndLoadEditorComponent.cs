

using Assets.Editor.Data;
using Assets.Scripts.Blocks.components.colors;
using Assets.Scripts.Data;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Assets.Editor.ColorBlockConfigurationEditor;

namespace Assets.Editor
{
    public abstract class SaveAndLoadEditorComponent
    {
        public event Action OnSaveAllPressed;
        public event Action OnLoadPressed;
        public event Action<IDataConfiguration> OnConfigurationSelected;

        protected IDataConfigurationCache _configurationCache;
        protected IDataConfiguration _currentConfigurationGroup;
        public Vector2 scrollPos;

        public virtual void Refresh()
        {
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

        public abstract void OnEnable();

        ///////////////////////////////////////////////////////////////////
        /// Private Methods
        ///////////////////////////////////////////////////////////////////
        protected void DrawSaveAndLoadToDiskButtons()
        {
            if (GUILayout.Button("Save All To Disk"))
            {
                _configurationCache.SaveToDisk();
                OnSaveAllPressed?.Invoke();
            }
            if (GUILayout.Button("Load From Disk"))
            {
                _configurationCache.LoadFromDisk();
                Debug.Log("Loaded From Disk");
                OnLoadPressed?.Invoke();
                _currentConfigurationGroup = (_configurationCache as ColorBlockConfigurationCache).Configurations.Count > 0 ? (_configurationCache as ColorBlockConfigurationCache).Configurations[0] : null;
                
                if(_currentConfigurationGroup != null)
                    OnConfigurationSelected?.Invoke(_currentConfigurationGroup);
            }
        }

        protected abstract void DrawSavedList();

        protected void TriggerConfigurationSelected(IDataConfiguration config)
        {
            OnConfigurationSelected?.Invoke(config);
        }

    }
}
