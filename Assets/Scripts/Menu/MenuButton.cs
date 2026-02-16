using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This button holds refrences to UI Panels to enable menu navigation
/// </summary>
public class MenuButton : MonoBehaviour
{

    [SerializeField] private GameObject goTo;

    public Button Button { get; set; }
    public GameObject GoTo { get => goTo; set => goTo = value; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
