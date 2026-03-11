using Assets.Scripts.Blocks.components;
using Assets.Scripts.Blocks.components.colors;
using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Systems.Goal
{
    internal class GoalManager : MonoBehaviour
    {

        public static GoalManager Instance;

        [SerializeField]
        private Transform _colorOverLayPrefab;

        [SerializeField]
        private Vector2 _location;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }

            Instance = this;
            
        }

        public void DrawOverlay(List<ColorBlockConfigurationData> blocks)
        {

            foreach (var block in blocks)
            {
                var colorOverlay = Instantiate(_colorOverLayPrefab, transform);
                colorOverlay.parent = transform;


                colorOverlay.localPosition = new Vector2(-block.x, -block.y) + _location;
                var goalBlockUX = colorOverlay.GetComponent<GoalBlockUX>();
                goalBlockUX.UpdateColor(block.color);
            }


        }


    
    }
}
