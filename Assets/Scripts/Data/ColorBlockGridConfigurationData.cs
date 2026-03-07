using Assets.Scripts.Blocks.components.colors;
using Assets.Scripts.Blocks.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.UI;

namespace Assets.Editor.Data
{
    [Serializable]
    internal class ColorBlockGridConfigurationData
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
    internal class ColorBLockGridNodeConfigurationData
    {
        public int x;
        public int y;
        public EntityType entityType;
    }


}
