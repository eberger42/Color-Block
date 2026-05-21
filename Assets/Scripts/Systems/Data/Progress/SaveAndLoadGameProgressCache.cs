using Assets.Scripts.Data;
using Assets.Scripts.Tools.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Systems.Data.Progress
{
    public class SaveAndLoadGameProgressCache : IDataConfigurationCache
    {

        private readonly string DATABASEPATH = Path.Combine(Application.persistentDataPath, "Data.json");

        public List<LevelConfigurationData> Configurations { get => _collection.configurations; }

        private DataConfigurationCollection<LevelConfigurationData> _collection = new();


        void IDataConfigurationCache.UpdateConfiguration(IDataConfiguration configuration)
        {
            if (!Configurations.Any(c => (c as IDataConfiguration).id == configuration.id))
            {
                Configurations.Add((LevelConfigurationData)configuration);
            }
            else
            {
                var index = Configurations.FindIndex(c => (c as IDataConfiguration).id == configuration.id);
                Configurations[index] = (LevelConfigurationData)configuration;
            }
        }

        void IDataConfigurationCache.DeleteConfiguration(string id)
        {
            var index = Configurations.FindIndex(c => (c as IDataConfiguration).id == id);
            if (index != -1)
            {
                Configurations.RemoveAt(index);
            }
        }

        void IDataConfigurationCache.SaveToDisk()
        {
            _collection.configurations = Configurations;
            SaveAndLoadConfigurationDataFiles.SaveToDisk(_collection, DATABASEPATH);
        }

        void IDataConfigurationCache.LoadFromDisk()
        {
            _collection = SaveAndLoadConfigurationDataFiles.LoadFromDisk<DataConfigurationCollection<LevelConfigurationData>>(DATABASEPATH);
        }
        IDataConfiguration IDataConfigurationCache.GetConfigurationDataByID(string id)
        {
            return Configurations.FirstOrDefault(x => (x as IDataConfiguration)?.id == id);
        }
    }
}
