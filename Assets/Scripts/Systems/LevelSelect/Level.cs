using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Systems.LevelSelect
{

    [Serializable]
    public class Level
    {
        private LevelData levelData;

        public string LevelName => levelData.levelName;
        public string LevelID => levelData.levelId;
        public bool Unlocked => levelData.unlocked;
        public bool CompletionStatus => levelData.completionStatus;

        public Level(LevelData levelData)
        {
            this.levelData = levelData;
        }

    }
}
