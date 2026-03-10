using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Systems.Data
{
    [System.Serializable]
    public class LevelStateData
    {
        [SerializeField]
        public string levelName;
        [SerializeField]
        public string levelId;
        [SerializeField]
        public bool unlocked;
        [SerializeField]
        public bool completionStatus;

    }

    [System.Serializable]
    public class LevelDataList
    {
        [SerializeField]
        public List<LevelStateData> levels = new List<LevelStateData>();


    }




}

