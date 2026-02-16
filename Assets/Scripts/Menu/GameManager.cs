
using UnityEngine;

public class GameManager : MonoBehaviour
{

    #region  Singleton

    public static GameManager instance;

    private void Awake()
    {

        if (instance != null)
        {
            Debug.LogWarning("GameManager already exists");
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);

    }
    #endregion

}
