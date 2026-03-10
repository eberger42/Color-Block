
using Assets.Scripts.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Grid.components
{
    public class PuzzleGridLoader : GridLoader
    {
    }

    [Serializable]
    public class PuzzleGridConfiguration : GridConfiguration
    {
        private string _test;
        public PuzzleGridConfiguration() : base()
        {
        }
        public PuzzleGridConfiguration(ColorBlockGridConfigurationData configurationData) : base(configurationData)
        {

        }
    }

}
