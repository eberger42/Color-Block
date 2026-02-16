using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelect : MonoBehaviour
{

    [SerializeField]
    private Transform levelParent;

    [SerializeField]
    private Transform levelButtonPrefab;

    [SerializeField]
    private SceneController sceneController;

    private void Start()
    {
        sceneController = FindObjectOfType<SceneController>();
        SaveManager.instance.OnLoadGame += LoadLevelData;
        SaveManager.instance.Load();
    }

    private void OnDestroy()
    {
        SaveManager.instance.OnLoadGame -= LoadLevelData;
    }
    public void LoadLevelData()
    {
        Debug.Log("Here");
        var levels = SaveData.current.levelData.levels;
        int i = 1;
        foreach(var level in levels)
        {
            var gameObject = Instantiate(levelButtonPrefab, levelParent);
            gameObject.GetComponent<LevelButton>().Initialize(level, sceneController, i);
            i++;
        }

    }

    public void LoadLevel(string levelName)
    {
        SceneController.instance.LoadScene(levelName);
    }
}
