
using Assets.Scripts.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Grid.components
{
    [Serializable]
    public class PuzzleGridConfiguration : GridConfiguration
    {
        public PuzzleGridConfiguration(LevelConfigurationData configurationData) : base(configurationData)
        {

        }
    }

}
