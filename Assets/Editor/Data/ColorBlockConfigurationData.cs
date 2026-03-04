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
    public class ColorBlockGroupConfigurationData
    {

        public string name;
        public string id;
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
