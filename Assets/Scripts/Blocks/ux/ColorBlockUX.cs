using Assets.Scripts.Blocks.components;
using Assets.Scripts.Blocks.interfaces;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Blocks.ux
{

    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(ColorBlock))]
    public class ColorBlockUX : MonoBehaviour
    {

        private SpriteRenderer _spriteRenderer;
        private ColorBlock _colorBlock;
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _colorBlock = GetComponent<ColorBlock>();

            _colorBlock.OnColorUpdated += UpdateColor;
        }

        private void UpdateColor(IBlockColor color)
        {
            _spriteRenderer.color = color.GetColor();
        }
    }
}