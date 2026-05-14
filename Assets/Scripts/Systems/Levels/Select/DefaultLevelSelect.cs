
using Assets.Scripts.Systems.Data;
using Assets.Scripts.Systems.LevelSelect;
using UnityEngine;
using static UnityEngine.Rendering.STP;

namespace Assets.Scripts.Systems.Levels.Select
{
    public sealed class DefaultLevelSelect : MonoBehaviour
    {

        private LevelSelectManager _levelSelectManager;
        private ColorBlockGridDataAccessor _colorBlockGridDataAccessor;

        void Start()
        {
            _levelSelectManager = LevelSelectManager.Instance;
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
