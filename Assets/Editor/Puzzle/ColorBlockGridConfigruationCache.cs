using Assets.Editor.Data;
using Assets.Editor.Tools;
using Assets.Scripts.Blocks.components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Data
{

    internal class ColorBlockGridConfigruationCache : IDataConfigurationCache
    {
        private readonly string DATABASEPATH = "Assets/Editor/PuzzleLevels.json";

        public List<ColorBlockGridConfigurationData> Configurations { get=> _collection.configurations; }

        private DataConfigurationCollection<ColorBlockGridConfigurationData> _collection = new();

        public void SaveConfiguration(ColorBlockGridConfigurationData configuration)
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

        void IDataConfigurationCache.UpdateConfiguration(IDataConfiguration configuration)
        {
            if (!Configurations.Any(c => (c as IDataConfiguration).id == configuration.id))
            {
                Configurations.Add((ColorBlockGridConfigurationData)configuration);
            }
            else
            {
                var index = Configurations.FindIndex(c => (c as IDataConfiguration).id == configuration.id);
                Configurations[index] = (ColorBlockGridConfigurationData)configuration;
            }
        }

        void IDataConfigurationCache.SaveToDisk()
        {
            _collection.configurations = Configurations;
            SaveAndLoadEditorFiles.SaveToDisk(_collection, DATABASEPATH);
        }

        void IDataConfigurationCache.LoadFromDisk()
        {
            _collection = SaveAndLoadEditorFiles.LoadFromDisk<DataConfigurationCollection<ColorBlockGridConfigurationData>>(DATABASEPATH);
        }
    }

}
