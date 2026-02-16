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

    [SerializeField]
    private string levelName;
    [SerializeField]
    private bool unlocked;
    [SerializeField]
    private bool completionStatus;

    private SceneController sceneController;
    private int index;

    public Button Button { get; set; }

    // Start is called before the first frame update
    void Awake()
    {

    }

    public void Initialize(Level level, SceneController sceneController, int index)
    {
        levelName = level.levelName;
        unlocked = level.unlocked;
        completionStatus = level.completionStatus;

        this.sceneController = sceneController;
        this.index = index;

        Button = GetComponent<Button>();
        Button.GetComponentInChildren<TextMeshProUGUI>().text = index + "";
        Button.onClick.AddListener(LoadLevel);
        Button.interactable = unlocked;
    }

    public void LoadLevel()
    {

        SceneController.instance.LoadScene(levelName);
    }

}
