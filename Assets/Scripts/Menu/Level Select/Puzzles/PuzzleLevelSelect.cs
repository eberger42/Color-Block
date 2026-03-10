using Assets.Scripts.Data;
using Assets.Scripts.Systems.LevelSelect;
using UnityEngine;

public class PuzzleLevelSelect : MonoBehaviour
{

    private ColorBlockGridConfigruationCache _configurationCache;

    [SerializeField] private Transform _buttonPrefab;
    [SerializeField] private Transform _buttonContainer;

    private void Awake()
    {
        _configurationCache = new ColorBlockGridConfigruationCache();
        (_configurationCache as IDataConfigurationCache).LoadFromDisk();

        foreach (var config in (_configurationCache as ColorBlockGridConfigruationCache).Configurations)
        {
            //TempLevel Data creation, need to save this data somehwere eventually

            var levelData = new LevelData()
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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
