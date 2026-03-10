using Assets.Scripts.Data;
using Assets.Scripts.Grid.components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Systems.LevelSelect
{
    public class LevelSelectManager : MonoBehaviour
    {

        public static LevelSelectManager Instance { get; private set; }
        public PuzzleGridConfiguration GridConfiguration { get => _gridConfiguration; }

        private ColorBlockGridConfigruationCache _colorBlockGridConfigruationCache;


        private Level _currentLevel;

        [SerializeField]
        private PuzzleGridConfiguration _gridConfiguration;



        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
                _colorBlockGridConfigruationCache = new ColorBlockGridConfigruationCache();
                (_colorBlockGridConfigruationCache as IDataConfigurationCache).LoadFromDisk();
            }
        }

        public void SelectLevel(Level level)
        {
            _currentLevel = level;
            var gridConfigurationData = _colorBlockGridConfigruationCache.Configurations.Find(gc => gc.id == level.LevelID);
            _gridConfiguration = new PuzzleGridConfiguration(gridConfigurationData);
        }   


    }
}
