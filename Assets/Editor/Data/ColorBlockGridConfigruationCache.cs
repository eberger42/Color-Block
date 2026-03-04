using Assets.Editor.Data;
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
        public List<IDataConfiguration> configurations = new();
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

    }

}
