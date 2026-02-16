using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public delegate void SaveGameCallback();
    public event SaveGameCallback OnSaveGame;

    public delegate void LoadGameCallback();
    public event LoadGameCallback OnLoadGame;

    #region Singleton
    public static SaveManager instance;

    protected void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of SaveManager found");
            Destroy(gameObject);
            return;
        }

        instance = this;

    }
    #endregion

    private void Start()
    {
    }

    private void Update()
    {
    }

    public void Load()
    {
        SaveData.current = (SaveData)SerializationManager.LoadJSON("LevelSave");

        Debug.Log("Invoking");
        OnLoadGame?.Invoke();

    }

    public void Save()
    {
        if (OnSaveGame != null)
            OnSaveGame();

        OnSave();

    }

    public void OnSave()
    {
        SerializationManager.SaveJSON("LevelSave", SaveData.current);
    }

    public string[] saveFiles;
    public void GetLoadFiles()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/saves/"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/saves/");
        }

        saveFiles = Directory.GetFiles(Application.persistentDataPath + "/saves/");
    }
}
