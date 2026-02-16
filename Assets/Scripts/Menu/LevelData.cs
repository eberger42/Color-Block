using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    [SerializeField]
    public List<Level> levels = new List<Level>();

    public LevelData()
    {
    }

}

[System.Serializable]
public class Level
{

    [SerializeField]
    public string levelName;
    [SerializeField]
    public bool unlocked;
    [SerializeField]
    public bool completionStatus;

}