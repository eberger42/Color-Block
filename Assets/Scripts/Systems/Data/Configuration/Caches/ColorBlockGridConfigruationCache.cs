
using Assets.Scripts.Tools.Data;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Data
{

    public class ColorBlockGridConfigruationCache : IDataConfigurationCache
    {
        private readonly string DATABASEPATH = "Assets/Editor/PuzzleLevels.json";

        public List<LevelConfigurationData> Configurations { get=> _collection.configurations; }

        private DataConfigurationCollection<LevelConfigurationData> _collection = new();

        public void SaveConfiguration(LevelConfigurationData configuration)
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
                Configurations.Add((LevelConfigurationData)configuration);
            }
            else
            {
                var index = Configurations.FindIndex(c => (c as IDataConfiguration).id == configuration.id);
                Configurations[index] = (LevelConfigurationData)configuration;
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
