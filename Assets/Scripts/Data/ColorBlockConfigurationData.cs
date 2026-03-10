
using Assets.Scripts.Blocks.interfaces;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Data
{
    [Serializable]
    public class ColorBlockGroupConfigurationData : IDataConfiguration
    {
        public ColorBlockGroupConfigurationData() { }

        public string id { get; set; }
        public string name { get; set; }
        public List<ColorBlockConfigurationData> blocks;
        public GridPosition pivotPosition;

    }

    [Serializable]
    public class ColorBlockConfigurationData
    {
        public int x;
        public int y;
        public ColorType color;
    }

}
