using Assets.Editor.Data;
using Assets.Scripts.Blocks.components;
using Assets.Scripts.Blocks.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Grid.components
{
    internal class PuzzleGridLoader : GridLoader
    {
    }

    internal class PuzzleGridConfiguration : GridConfiguration
    {
        private Queue<ColorBlockConfiguration> blockConfigurations = new Queue<ColorBlockConfiguration>();
        public PuzzleGridConfiguration(ColorBlockGridConfigurationData configurationData) : base(configurationData)
        {

        }
    }


    internal class ColorBlockConfiguration
    {

    }
}
