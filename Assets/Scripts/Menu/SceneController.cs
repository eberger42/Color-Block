using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Scenes
{
    MainMenu = 0, Tutorial = 1,
    Level_1 = 11, Level_2 = 12, Level_3 = 13, Level_4 = 14, Level_5 = 15,
}


public class SceneController : MonoBehaviour
{

    public static SceneController instance;

    [SerializeField]
    private Image screenCover;

    [SerializeField]
    private Animator transition;

    [SerializeField]
    private float transitionTime = 1f;



    public delegate void SceneLoadedCallback(Scenes scene);
    public event SceneLoadedCallback OnSceneLoaded; 
    
    


    private void Awake()
    {

        #region  Singleton
        if (instance != null)
        {
            Debug.LogWarning("SceneController already exists");
            Destroy(gameObject);
            return;
        }
        instance = this;
        #endregion

    }


    private void Start()
    {
        SceneManager.sceneLoaded += SceneLoaded;
    }

    public void LoadScene(string to)
    {

        StartCoroutine(wait(to));

        IEnumerator wait(string to)
        {
            transition.SetTrigger("Start");
            yield return new WaitForSecondsRealtime(transitionTime);

            if (GetCurrentScene().ToString() != to)
                SceneManager.LoadScene(to);
        }

    }
    
    public void LoadScene(Scenes to)
    {

        StartCoroutine(wait(to));

        IEnumerator wait(Scenes to)
        {
            transition.SetTrigger("Start");
            yield return new WaitForSecondsRealtime(transitionTime);

            if (GetCurrentScene() != to)
                SceneManager.LoadScene(to.ToString());
        }

    }
    
    /// <summary>
     /// This gets called automatically whenever unity loads a new scene.
     /// In this instance, it gets called after the screen fades to black
     /// </summary>
     /// <param name="scene"></param>
     /// <param name="mode"></param>
    void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (OnSceneLoaded != null)
            OnSceneLoaded(GetCurrentScene());

        if(transition != null)
            transition.SetTrigger("End");


    }

    /// <summary>
    /// Returns the enum equivelant of the current scene
    /// </summary>
    /// <returns></returns>
    public Scenes GetCurrentScene()
    {
        return ((Scenes)System.Enum.Parse(typeof(Scenes), SceneManager.GetActiveScene().name));
    }

    private void SetScreenImage(float value)
    {
        screenCover.color = new Color(0, 0, 0, 1 - value);
    }

}
