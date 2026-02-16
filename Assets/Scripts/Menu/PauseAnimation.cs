using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseAnimation : MonoBehaviour
{

    [SerializeField]
    private PauseManager pauseManager;

    [SerializeField]
    private GameObject pauseMenu;

    [SerializeField]
    private Button defaultButton;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        pauseManager.OnPause += PauseToggle;
    }

    private void OnDestroy()
    {
        pauseManager.OnPause -= PauseToggle;
    }


    private void PauseToggle(bool isPaused)
    {
        Debug.Log("Animator Pause");
        if (isPaused)
        {
            animator.SetTrigger("Start");
            StartCoroutine(wait());
        }
        else
        {
            animator.SetTrigger("End");

        }

        IEnumerator wait()
        {
            yield return new WaitForEndOfFrame();
            EventSystem.current.SetSelectedGameObject(defaultButton.gameObject);

        }

    }
}
