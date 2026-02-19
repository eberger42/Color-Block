using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public delegate void PauseEvent(bool isPaused);
    public PauseEvent OnPause;
    
    private bool gamePaused = false;

    private void Start()
    {
        Time.timeScale = 1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }
    public void PauseGame()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
            return;

        if (!gameObject.activeInHierarchy)
                return;

        gamePaused = !gamePaused;
        OnPause?.Invoke(gamePaused);

        if (gamePaused)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    
    }

    public void ResetPause()
    {
        Time.timeScale = 1;
    }

}
