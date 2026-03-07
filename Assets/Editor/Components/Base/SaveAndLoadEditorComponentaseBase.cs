

using Assets.Editor.Data;
using Assets.Scripts.Blocks.components.colors;
using Assets.Scripts.Data;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Assets.Editor.ColorBlockConfigurationEditor;

namespace Assets.Editor.Components
{
    internal abstract class SaveAndLoadEditorComponentaseBase
    {
        private readonly IUseSaveAndLoadEditorComponent _listener;

        public event Action OnSaveAllPressed;
        public event Action OnLoadPressed;
        public event Action<IDataConfiguration> OnConfigurationSelected;

        protected IDataConfigurationCache _configurationCache;
        protected IDataConfiguration _currentConfigurationGroup;
        public Vector2 scrollPos;

        public SaveAndLoadEditorComponentaseBase(IUseSaveAndLoadEditorComponent listener)
        {
            _listener = listener;
        }

        public virtual void Refresh()
        {
        }
        public void OnGUI()
        {
            GUILayout.BeginVertical(GUILayout.Width(250));
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            DrawSaveAndLoadToDiskButtons();
            DrawSaveAndNew();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            DrawSavedList();
            GUILayout.EndVertical();
        }

        public abstract void OnEnable();

        public abstract void CreateNewConfiguration();

        ///////////////////////////////////////////////////////////////////
        /// Private Methods
        ///////////////////////////////////////////////////////////////////
        protected void DrawSaveAndLoadToDiskButtons()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save All To Disk"))
            {
                _configurationCache.SaveToDisk();
                OnSaveAllPressed?.Invoke();
            }
            if (GUILayout.Button("Load From Disk"))
            {
                _configurationCache.LoadFromDisk();
                OnLoadPressed?.Invoke();
                PostDataLoad();
                Refresh();  
            }
            GUILayout.EndHorizontal();
        }

        protected void DrawSaveAndNew()
        {
            GUILayout.Space(2);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save Configuration"))
            {
                _listener.SaveCurrentConfiguration();
            }
            if (GUILayout.Button("New Configuration"))
            {
                CreateNewConfiguration();
            }
            GUILayout.EndHorizontal();


        }

        protected abstract void DrawSavedList();

        /// <summary>
        /// Used to run code after data is loaded from disk, such as setting the current configuration group and triggering events to load that data into the editor.
        /// </summary>
        protected abstract void PostDataLoad();

        protected void TriggerConfigurationSelected(IDataConfiguration config)
        {
            OnConfigurationSelected?.Invoke(config);
        }

    }

    internal interface IUseSaveAndLoadEditorComponent
    {
        void SaveCurrentConfiguration();
    }
}
