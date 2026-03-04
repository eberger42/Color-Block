using UnityEditor;
using UnityEngine;

public class PuzzleLevelEditor : EditorWindow
{


    [MenuItem("Tools/Puzzle Level Editor")]
    public static void Open()
    {
        GetWindow<PuzzleLevelEditor>("Puzzle Level Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Puzzle Level Editor", EditorStyles.boldLabel);
        if (GUILayout.Button("Create New Puzzle Level"))
        {
            // Logic to create a new puzzle level
            Debug.Log("Creating a new puzzle level...");
        }
        if (GUILayout.Button("Load Existing Puzzle Level"))
        {
            // Logic to load an existing puzzle level
            Debug.Log("Loading an existing puzzle level...");
        }
        // Additional UI elements for editing the puzzle level can be added here
    }
}

