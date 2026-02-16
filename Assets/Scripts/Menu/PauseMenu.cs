using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    

    public void QuitToMenu()
    {
        SceneController.instance.LoadScene(Scenes.MainMenu);
    }
}
