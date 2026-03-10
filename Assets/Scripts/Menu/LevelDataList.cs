using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelDataList
{
    [SerializeField]
    public List<LevelData> levels = new List<LevelData>();

    public LevelDataList()
    {
    }

}

[System.Serializable]
public class LevelData
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