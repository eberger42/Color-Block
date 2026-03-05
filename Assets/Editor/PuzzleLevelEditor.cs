using Assets.Editor.Components;
using Assets.Editor.Data;
using Assets.Scripts.Blocks.components.colors;
using Assets.Scripts.Grid.components;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Assets.Editor.ColorBlockConfigurationEditor;

public class PuzzleLevelEditor : EditorWindow
{

    private enum ViewMode
    {
        ColorPalette,
        EntityPalette,
    }

    //EditorComponents
    private PuzzleConfigSaveAndLoadComponent _saveLoadComponent;
    private ColorPaletteComponent _colorPaletteComponent;


    private ColorBLockGridNodeConfigurationData[,] grid;

    private string _puzzleName = "New Puzzle Level";
    private int _width = 0;
    private int _height = 0;

    //private UI Helpers
    private ViewMode _currentViewMode = ViewMode.ColorPalette;

    [MenuItem("Tools/Puzzle Level Editor")]
    public static void Open()
    {
        GetWindow<PuzzleLevelEditor>("Puzzle Level Editor");
    }

    ///////////////////////////////////////////////////////////////////////////
    /// Lifecycle Methods
    ///////////////////////////////////////////////////////////////////////////
    private void OnEnable()
    {
        grid = new ColorBLockGridNodeConfigurationData[_width, _height];
        _saveLoadComponent = new PuzzleConfigSaveAndLoadComponent(_width, _height);
        _saveLoadComponent.OnConfigurationSelected += LoadConfigurationIntoGrid;

        _saveLoadComponent.OnEnable();

        _colorPaletteComponent = new ColorPaletteComponent();
        _colorPaletteComponent.OnEnable();


    }


    private void OnDisable()
    {
        _saveLoadComponent.OnConfigurationSelected -= LoadConfigurationIntoGrid;
    }


    private void OnGUI()
    {
        GUILayout.BeginHorizontal();

        _saveLoadComponent.OnGUI();

        GUILayout.Space(10);

        GUILayout.BeginVertical();
        DrawPuzzleLevelSettings();
        DrawGrid();
        DrawSaveAndNew();
        GUILayout.EndVertical();
        DrawPalettes();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    //////////////////////////////////////////////////////////////////////////
    /// Private Methods
    //////////////////////////////////////////////////////////////////////////
    private void DrawGrid()
    {
        for (int y = 0; y < _height; y++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < _width; x++)
            {
                Color cellColor = Color.white;

                var oldColor = GUI.backgroundColor;

                GUI.backgroundColor = cellColor;

                if (GUILayout.Button("", GUILayout.Width(40), GUILayout.Height(40)))
                {
                    
                }
                // Restore original color
                GUI.backgroundColor = oldColor;

            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawSaveAndNew()
    {
        GUILayout.Space(10);

        if (GUILayout.Button("Save Configuration"))
        {
            SaveCurrentConfiguration();
        }
        if (GUILayout.Button("New Configuration"))
        {
            _saveLoadComponent.CreateNewConfiguration();
        }


    }

    private void DrawPuzzleLevelSettings()
    {
        GUILayout.Space(10);

        GUILayout.Label("Settings", EditorStyles.boldLabel);
        GUILayout.BeginVertical();

        _puzzleName = EditorGUILayout.TextField("", _puzzleName, GUILayout.MaxWidth(150));

        EditorGUI.BeginChangeCheck();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Width:", GUILayout.Width(45)); 
        _width = EditorGUILayout.IntField(_width, GUILayout.Width(40));
        GUILayout.Label("Height:", GUILayout.Width(45));
        _height = EditorGUILayout.IntField(_height, GUILayout.Width(40));
        GUILayout.EndHorizontal();
        if (EditorGUI.EndChangeCheck())
        {
            var newGrid = new ColorBLockGridNodeConfigurationData[_width, _height];

            for(int y = 0; y < Mathf.Min(_height, grid.GetLength(1)); y++)
            {
                for (int x = 0; x < Mathf.Min(_width, grid.GetLength(0)); x++)
                {
                    newGrid[x, y] = grid[x, y];
                }
            }
            grid = newGrid;
        }


        GUILayout.EndVertical();
    }

    private void DrawPalettes()
    {

        void DrawColorPalette()
        {
            _colorPaletteComponent.OnGUI();
        }
        void DrawEntityPalette()
        {
            GUILayout.Label("Entity Palette");
        }

        GUILayout.Space(10);
        GUILayout.Label("Palette", EditorStyles.boldLabel);
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();

        if(GUILayout.Toggle(_currentViewMode == ViewMode.ColorPalette, "Color Palette", "Button"))
            _currentViewMode = ViewMode.ColorPalette;
        if (GUILayout.Toggle(_currentViewMode == ViewMode.EntityPalette, "Entity Palette", "Button"))
            _currentViewMode = ViewMode.EntityPalette;

        GUILayout.EndHorizontal();  

        switch(_currentViewMode)
        {
            case ViewMode.ColorPalette:
                DrawColorPalette();
                break;
            case ViewMode.EntityPalette:
                DrawEntityPalette();
                break;
        }

        GUILayout.EndVertical();
    }
    private void LoadConfigurationIntoGrid(IDataConfiguration config)
    {
        _puzzleName = config.name;
        _width = (config as ColorBlockGridConfigurationData).width;
        _height = (config as ColorBlockGridConfigurationData).height;

        grid = new ColorBLockGridNodeConfigurationData[_width, _height];

        foreach (var block in (config as ColorBlockGridConfigurationData).gridNodes)
        {
            if (block.x < _width && block.y < _height)
                grid[block.x, block.y] = new ColorBLockGridNodeConfigurationData { x = block.x, y = block.y };
        }

    }
    private void SaveCurrentConfiguration()
    {
        var blocks = new List<ColorBLockGridNodeConfigurationData>();
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                if (grid[x, y] != null)
                {
                    blocks.Add(grid[x, y]);
                }
            }
        }

        _saveLoadComponent.UpdateConfiguration(blocks, _puzzleName, _width, _height);
        Repaint();
    }
}

