using Assets.Editor.Tools;
using Assets.Scripts.Blocks.components;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.Data
{
    [Serializable]
    public class ColorBlockConfigurationCollection
    {
        public List<ColorBlockGroupConfigurationData> configurations = new();
    }

    public class EditorConfigruationCache<T> : IDataConfigurationCache<ColorBlockGroupConfigurationData>
    {
        private static readonly string DATABASEPATH = "Assets/Editor/TetrominoDatabase.json";

        public static List<ColorBlockGroupConfigurationData> Configurations { get => _collection.configurations; }

        private static ColorBlockConfigurationCollection _collection = new();

        public EditorConfigruationCache(string path)
        {
        }


        void IDataConfigurationCache<ColorBlockGroupConfigurationData>.UpdateConfiguration(ColorBlockGroupConfigurationData configuration)
        {
            if (!Configurations.Any(c => c.id == configuration.id))
            {
                Configurations.Add(configuration);
            }
            else
            {
                var index = Configurations.FindIndex(c => c.id == configuration.id);
                Configurations[index] = configuration;
            }
        }
        void IDataConfigurationCache<ColorBlockGroupConfigurationData>.SaveToDisk()
        {
            _collection.configurations = Configurations;
            SaveAndLoadEditorFiles.SaveToDisk(_collection, DATABASEPATH);

        }

        void IDataConfigurationCache<ColorBlockGroupConfigurationData>.LoadFromDisk()
        {
            _collection = SaveAndLoadEditorFiles.LoadFromDisk<ColorBlockConfigurationCollection>(DATABASEPATH);
           
        }


    }

}
