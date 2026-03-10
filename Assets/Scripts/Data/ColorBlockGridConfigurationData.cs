
using Assets.Scripts.Blocks.interfaces;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Data
{
    [Serializable]
    public class ColorBlockGridConfigurationData : IDataConfiguration
    {

        public string id { get; set; }
        public string name { get; set; }
        public int width { get; set; }
        public int height { get; set; }

        public List<ColorBlockConfigurationData> puzzleOverlay =new();
        public List<ColorBLockGridNodeConfigurationData> gridNodes =new();
        public List<string> queue = new();

    }

    [Serializable]
    public class ColorBLockGridNodeConfigurationData
    {
        public int x;
        public int y;
        public EntityType entityType;
    }


}
