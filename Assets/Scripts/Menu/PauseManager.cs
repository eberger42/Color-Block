using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{


    private PlayerInput playerInput;
    private PlayerInputManager playerInputManager;


    public delegate void PauseEvent(bool isPaused);
    public PauseEvent OnPause;
    
    private bool gamePaused = false;

    private void Start()
    {
        playerInputManager = PlayerInputManager.instance;
        playerInput = playerInputManager.PlayerInput;

        playerInputManager.OnEscape += PauseGame;

    }

    private void OnDestroy()
    {
        playerInputManager.OnEscape -= PauseGame;
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
            playerInput.SwitchCurrentActionMap("UI");
            Time.timeScale = 0;
        }
        else
        {
            playerInput.SwitchCurrentActionMap("Player");
            Time.timeScale = 1;
        }
    
    }

    public void ResetPause()
    {
        Time.timeScale = 1;
    }

}
