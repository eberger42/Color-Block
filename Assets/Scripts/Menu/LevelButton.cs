using Assets.Scripts.Systems.Data;
using Assets.Scripts.Systems.LevelSelect;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class holds the button component and the scene that the button targets
/// </summary>
public class LevelButton : MonoBehaviour
{

    private const Scenes SCENE_TO_LOAD = Scenes.DailyPuzzle;

    private Level _level;

    public Button Button { get; set; }

    // Start is called before the first frame update
    void Awake()
    {

    }

    public void Initialize(Level level)
    {
        _level = level;

        Button = GetComponent<Button>();
        Button.GetComponentInChildren<TextMeshProUGUI>().text = _level.LevelName;
        Button.onClick.AddListener(LoadLevel);
        Button.interactable = _level.Unlocked;
    }

    public void LoadLevel()
    {
        LevelSelectManager.Instance.SelectLevel(_level);
        SceneController.instance.LoadScene(SCENE_TO_LOAD);
    }

}
