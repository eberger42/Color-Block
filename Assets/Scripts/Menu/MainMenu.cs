using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    /// <summary>
    /// Button Event Handled in Unity
    /// </summary>

    private GameObject activePanelCard;

    [SerializeField]
    private GameObject titleCard;

    [SerializeField]
    private GameObject levelSelect;

    [SerializeField]
    private GameObject settings;


    private void Start()
    {
        activePanelCard = titleCard;
    }


    public void LoadGame()
    {
        SceneController.instance.LoadScene(Scenes.Level_1);
    }

    public void GoToSetttings()
    {
        activePanelCard.SetActive(false);
        activePanelCard = settings;
        settings.SetActive(true);
    }
    public void GoToLevelSelect()
    {
        activePanelCard.SetActive(false);
        activePanelCard = levelSelect;
        levelSelect.SetActive(true);
    }

    public void GoToMainMenu()
    {
        activePanelCard.SetActive(false);
        activePanelCard = titleCard;
        titleCard.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
