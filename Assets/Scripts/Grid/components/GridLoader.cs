
using Assets.Scripts.Data;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Grid.components
{
    public abstract class GridLoader : MonoBehaviour
    {



        private IEnumerator 


    }

    [Serializable]
    public class GridConfiguration
    {
        protected string _id;
        protected string _name;
        private int _width;
        private int _height;

        public int Width { get => _width; }
        public int Height { get => _height; }

        public GridConfiguration() { }

        public GridConfiguration(ColorBlockGridConfigurationData configurationData) 
        { 
            _id = configurationData.id;
            _name = configurationData.name;
            _width = configurationData.width;
            _height = configurationData.height;

        }


    }
}
