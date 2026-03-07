using Assets.Scripts.Blocks.components.colors;
using Assets.Scripts.Blocks.interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.UI;

namespace Assets.Editor.Data
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
