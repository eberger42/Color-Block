
using Assets.Scripts.Systems.Data;
using Assets.Scripts.Systems.LevelSelect;
using UnityEngine;
using static UnityEngine.Rendering.STP;

namespace Assets.Scripts.Systems.Levels.Select
{

    [RequireComponent(typeof(LevelSelectManager))]
    public sealed class DefaultLevelSelect : MonoBehaviour
    {

        private LevelSelectManager _levelSelectManager;
        private ColorBlockGridDataAccessor _colorBlockGridDataAccessor;


        private void Awake()
        {
            _levelSelectManager = GetComponent<LevelSelectManager>();


          
        }

        void Start()
        {
            _colorBlockGridDataAccessor = ColorBlockGridDataAccessor.Instance;

            var config = _colorBlockGridDataAccessor.GetColorBlockConfigurationDataByID("__Default");

            var levelData = new LevelStateData()
            {
                levelName = config.name,
                levelId = config.id,
                unlocked = true,
                completionStatus = false
            };

            _levelSelectManager.SelectLevel(new Level(levelData));
        }
    }
}
