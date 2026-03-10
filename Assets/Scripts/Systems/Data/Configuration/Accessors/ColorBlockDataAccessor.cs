
using Assets.Scripts.Data;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Systems.Data
{
    public class ColorBlockDataAccessor : MonoBehaviour
    {

        public static ColorBlockDataAccessor Instance { get; private set; }

        private static readonly ColorBlockConfigurationCache COLOR_BLOCK_CONFIG_CACHE = new ColorBlockConfigurationCache();


        private void Awake()
        {

            if (Instance != null)
            {
                Destroy(this.gameObject);
                return;
            }

            Instance = this;

            (COLOR_BLOCK_CONFIG_CACHE as IDataConfigurationCache).LoadFromDisk();

        }

        public ColorBlockGroupConfigurationData GetColorBlockConfigurationDataByID(string colorBlockId)
        {
            return (COLOR_BLOCK_CONFIG_CACHE as IDataConfigurationCache).GetConfigurationDataByID(colorBlockId) as ColorBlockGroupConfigurationData;
        }

        public List<ColorBlockGroupConfigurationData> GetAllConfigurations()
        {
            return COLOR_BLOCK_CONFIG_CACHE.Configurations;
        }
    }
}
