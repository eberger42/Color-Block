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
    [Serializable]
    public class ColorBlockGridConfigurationCollection
    {
        public List<ColorBlockGridConfigurationData> configurations = new();
    }

    public static class ColorBlockGridConfigruationCache
    {
        private static readonly string DATABASEPATH = "Assets/Editor/PuzzleLevels.json";

        public static List<ColorBlockGridConfigurationData> Configurations { get; private set; } = new();

        private static ColorBlockGridConfigurationCollection _collection = new();

        public static void SaveConfiguration(ColorBlockGridConfigurationData configuration)
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

        public static void SaveToDisk()
        {
            _collection.configurations = Configurations;

            string json = JsonUtility.ToJson(_collection, true);
            System.IO.File.WriteAllText(DATABASEPATH, json);
            AssetDatabase.Refresh();
        }

        public static void LoadFromDisk()
        {
            if (!System.IO.File.Exists(DATABASEPATH))
            {
                Configurations = new List<ColorBlockGridConfigurationData>();
                _collection = new ColorBlockGridConfigurationCollection();
                return;
            }

            string json = System.IO.File.ReadAllText(DATABASEPATH);
            _collection = JsonUtility.FromJson<ColorBlockGridConfigurationCollection>(json);

            Configurations = _collection.configurations ?? new List<ColorBlockGridConfigurationData>();
        }
    }

}
