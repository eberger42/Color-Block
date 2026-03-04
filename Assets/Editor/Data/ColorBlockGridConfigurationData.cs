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
    public class ColorBlockGridConfigurationData
    {

        public string name;
        public string id;
        public List<ColorBlockConfigurationData> puzzleOverlay;
        public List<ColorBLockGridNodeConfigurationData> gridNodes;

    }

    [Serializable]
    public class ColorBLockGridNodeConfigurationData
    {
        public int x;
        public int y;
        public EntityType entityType;
    }


}
