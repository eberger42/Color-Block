using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadScene : MonoBehaviour
{
    [SerializeField]
    private Scenes levelToLoad;

    public void LoadLevel()
    {
        SceneController.instance.LoadScene(levelToLoad);
    }

}
