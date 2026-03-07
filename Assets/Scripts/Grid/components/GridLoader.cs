using Assets.Editor.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Grid.components
{
    internal abstract class GridLoader : MonoBehaviour
    {



    }

    internal abstract class GridConfiguration
    {

        protected readonly string _id;
        protected readonly string _name;
        protected readonly int _width;
        protected readonly int _height;


        public GridConfiguration(ColorBlockGridConfigurationData configurationData) 
        { 
            _id = configurationData.id;
            _name = configurationData.name;
            _width = configurationData.width;
            _height = configurationData.height;

        }


    }
}
