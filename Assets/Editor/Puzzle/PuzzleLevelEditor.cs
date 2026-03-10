using Assets.Editor.Components;
using Assets.Editor.Puzzle;
using Assets.Scripts.Data;
using UnityEditor;
using UnityEngine;

public class PuzzleLevelEditor : EditorWindow, IUseSaveAndLoadEditorComponent
{

    private enum PalleteViewMode
    {
        ColorPalette,
        EntityPalette,
    }

    private enum  EditorViewMode
    {
        GridEdit,
        BlockQueueEdit
    }

    //EditorComponents
    private PuzzleConfigSaveAndLoadComponent _saveLoadComponent;
    private ColorPaletteGridComponent _colorPaletteGridComponent;
    private EntityGridComponent _entityGridComponent;
    private ColorBlockQueueComponent _colorBlockQueueComponent;
    
    //Selector Comps
    private readonly EnumSelectorComponent<PalleteViewMode> _paletteViewModeSelector = new();
    private readonly EnumSelectorComponent<EditorViewMode> _editorViewModeSelector = new();

    private string _puzzleName = "New Puzzle Level";
    private int _width = 0;
    private int _height = 0;


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

        _colorPaletteGridComponent = new ColorPaletteGridComponent(_width, _height);
        _colorPaletteGridComponent.OnEnable();

        _entityGridComponent = new EntityGridComponent(_width, _height);
        _entityGridComponent.OnEnable();

        _colorBlockQueueComponent = new ColorBlockQueueComponent();
        _colorBlockQueueComponent.OnEnable();

        _saveLoadComponent = new PuzzleConfigSaveAndLoadComponent(this, _width, _height);
        _saveLoadComponent.OnConfigurationSelected += LoadConfigurationIntoGrid;
        _saveLoadComponent.OnEnable();


        _paletteViewModeSelector.CurrentViewMode = EnumSelectorComponent<PalleteViewMode>.ViewMode.Buttons;
        _editorViewModeSelector.CurrentViewMode = EnumSelectorComponent<EditorViewMode>.ViewMode.Buttons;

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
        _editorViewModeSelector.OnGUI();
        GUILayout.BeginHorizontal();

        switch (_editorViewModeSelector.SelectedValue)
        {
            case EditorViewMode.GridEdit:
                GUILayout.BeginVertical();
                _paletteViewModeSelector.OnGUI();
                DrawPuzzleLevelSettings();
                DrawGrid();
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                break;
            case EditorViewMode.BlockQueueEdit:
                _colorBlockQueueComponent.OnGUI();
                break;
        }
        
        GUILayout.EndHorizontal();

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

    }

    //////////////////////////////////////////////////////////////////////////
    /// Private Methods
    //////////////////////////////////////////////////////////////////////////
    private void DrawGrid()
    {

        switch (_paletteViewModeSelector.SelectedValue)
        {
            case PalleteViewMode.ColorPalette:
                _colorPaletteGridComponent.OnGUI();
                break;
            case PalleteViewMode.EntityPalette:
                _entityGridComponent.OnGUI();
                break;
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
            _colorPaletteGridComponent.UpdateGridSize(_width, _height);
            _entityGridComponent.UpdateGridSize(_width, _height);
        }


        GUILayout.EndVertical();
    }

    void LoadConfigurationIntoGrid(IDataConfiguration config)
    {
        _puzzleName = config.name;
        _width = (config as ColorBlockGridConfigurationData).width;
        _height = (config as ColorBlockGridConfigurationData).height;

        _entityGridComponent.LoadConfigurationIntoGrid(_width, _height, (config as ColorBlockGridConfigurationData).gridNodes);
        _colorPaletteGridComponent.LoadConfigurationIntoGrid(_width, _height, (config as ColorBlockGridConfigurationData).puzzleOverlay);
        _colorBlockQueueComponent.LoadConfigurationIntoQueue((config as ColorBlockGridConfigurationData).queue);

    }
    void IUseSaveAndLoadEditorComponent.SaveCurrentConfiguration()
    {
        var blocks = _entityGridComponent.GetGridConfiguration();
        var overlay = _colorPaletteGridComponent.GetGridConfiguration();
        var queue = _colorBlockQueueComponent.GetQueueConfiguration();

        _saveLoadComponent.UpdateConfiguration(blocks, overlay, queue, _puzzleName, _width, _height);
        Repaint();
    }

}

