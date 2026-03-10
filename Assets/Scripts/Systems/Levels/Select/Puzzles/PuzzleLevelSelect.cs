using Assets.Scripts.Data;
using Assets.Scripts.Systems.Data;
using Assets.Scripts.Systems.LevelSelect;
using UnityEngine;

public class PuzzleLevelSelect : LevelSelectBase
{

    private ColorBlockGridDataAccessor _colorBlockGridDataAccessor;


    protected override void Start()
    {
        _colorBlockGridDataAccessor = ColorBlockGridDataAccessor.Instance;
        base.Start();
    }

    public override void LoadLevelData()
    {
        var configData = _colorBlockGridDataAccessor.GetAllColorBlockConfigurationData();

        foreach (var config in configData)
        {
            //TempLevel Data creation, need to save this data somehwere eventually

            var levelData = new LevelStateData()
            {
                levelName = config.name,
                levelId = config.id,
                unlocked = true,
                completionStatus = false
            };

            var button = Instantiate(_buttonPrefab, _buttonContainer);
            var buttonComponent = button.GetComponent<LevelButton>();
            buttonComponent.Initialize(new Level(levelData));
            button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = config.name;

        }
    }

}
