
using Assets.Scripts.Data;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Systems.Data
{
    public class ColorBlockGridDataAccessor : MonoBehaviour
    {

        public static ColorBlockGridDataAccessor Instance { get; private set; }

        static readonly ColorBlockGridConfigruationCache CONFIG_CACHE = new ColorBlockGridConfigruationCache();


        void Awake()
        {

            if(Instance != null)
            {
                Destroy(this.gameObject);
                return;
            }
            
            Instance = this;

            (CONFIG_CACHE as IDataConfigurationCache).LoadFromDisk();

        }

        public LevelConfigurationData GetColorBlockConfigurationDataByID(string id)
        {
            return (CONFIG_CACHE as IDataConfigurationCache).GetConfigurationDataByID(id) as LevelConfigurationData;
        }

        public List<LevelConfigurationData> GetAllColorBlockConfigurationData()
        {
            return CONFIG_CACHE.Configurations;
        }

    }
}
