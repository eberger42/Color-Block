using Assets.Scripts.Blocks.components;
using Assets.Scripts.Blocks.components.colors;
using Assets.Scripts.Blocks.interfaces;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Systems.Goal
{

    [RequireComponent(typeof(SpriteRenderer))]
    public class GoalBlockUX : MonoBehaviour
    {

        private SpriteRenderer _spriteRenderer;


        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();

        }

        public void UpdateColor(ColorType colorType)
        {

            var color = BlockColor.ColorTypeToColorMap[colorType];


            _spriteRenderer.material.SetColor("_BaseColor", color);
            _spriteRenderer.material.SetColor("_FillColor", color);

        }

    }
}