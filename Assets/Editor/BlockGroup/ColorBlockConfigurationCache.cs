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
    public class DataConfigurationCollection<T> where T : IDataConfiguration, new()
    {
        public List<T> configurations = new();
    }

    public class ColorBlockConfigurationCache : IDataConfigurationCache
    {
        private readonly string DATABASEPATH = "Assets/Editor/ColorBlockConfigurations.json";

        public List<ColorBlockGroupConfigurationData> Configurations { get => _collection.configurations; }

        private DataConfigurationCollection<ColorBlockGroupConfigurationData> _collection = new();


        void IDataConfigurationCache.UpdateConfiguration(IDataConfiguration configuration) 
        {
            if (!Configurations.Any(c => (c as IDataConfiguration).id == configuration.id))
            {
                Configurations.Add((ColorBlockGroupConfigurationData)configuration);
            }
            else
            {
                var index = Configurations.FindIndex(c => (c as IDataConfiguration).id == configuration.id);
                Configurations[index] = (ColorBlockGroupConfigurationData)configuration;
            }

           
        }
        void IDataConfigurationCache.SaveToDisk()
        {
            _collection.configurations = Configurations;
            SaveAndLoadEditorFiles.SaveToDisk(_collection, DATABASEPATH);
        }

        void IDataConfigurationCache.LoadFromDisk()
        {
            _collection = SaveAndLoadEditorFiles.LoadFromDisk<DataConfigurationCollection<ColorBlockGroupConfigurationData>>(DATABASEPATH);
           
        }


    }

}
