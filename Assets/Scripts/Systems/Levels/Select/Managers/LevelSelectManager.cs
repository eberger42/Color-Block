using Assets.Scripts.Data;
using Assets.Scripts.Grid.components;
using Assets.Scripts.Systems.Data;
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

        private Level _currentLevel;

        public Level CurrentLevel { get => _currentLevel; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this.gameObject);
                return;
            }

            Instance = this;
            
        }

        public void SelectLevel(Level level)
        {
            _currentLevel = level;
        }   


    }
}
