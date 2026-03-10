using Assets.Scripts.Data;
using Assets.Scripts.Grid.components;
using Assets.Scripts.Systems.Data;
using System;

namespace Assets.Scripts.Systems.LevelSelect
{

    [Serializable]
    public class Level
    {
        private LevelStateData levelStateData;
        private LevelConfigurationData? _levelConfigData;

        //Properties
        public string LevelName => levelStateData.levelName;
        public string LevelID => levelStateData.levelId;
        public bool Unlocked => levelStateData.unlocked;
        public bool CompletionStatus => levelStateData.completionStatus;

        public Level(LevelStateData levelData)
        {
            this.levelStateData = levelData;
        }

        public void SetLevelConfigurationData(LevelConfigurationData configData)
        {
            _levelConfigData = configData;
        }

        public PuzzleGridConfiguration GetGridConfiguration()
        {
            if(_levelConfigData == null)
            {
                throw new InvalidOperationException("Level configuration data has not been set for this level.");
            }

            return new PuzzleGridConfiguration(_levelConfigData);
        }

    }
}
