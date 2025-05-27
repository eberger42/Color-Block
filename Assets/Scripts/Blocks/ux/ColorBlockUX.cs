using Assets.Scripts.Blocks.components;
using Assets.Scripts.Blocks.components.colors;
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

            (_colorBlock as IBlock).OnColorUpdated += UpdateColor;

        }

        private void UpdateColor(IBlockColor color)
        {

            var colorType = color.GetColorType();

            if(colorType == ColorType.Red)
                _spriteRenderer.sprite = BlockSpriteCache.Instance.RedBlockSprite;
            else if (colorType == ColorType.Blue)
                _spriteRenderer.sprite = BlockSpriteCache.Instance.BlueBlockSprite;
            else if (colorType == ColorType.Yellow)
                _spriteRenderer.sprite = BlockSpriteCache.Instance.YellowBlockSprite;
            else if (colorType == ColorType.Green)
                _spriteRenderer.sprite = BlockSpriteCache.Instance.GreenBlockSprite;
            else if (colorType == ColorType.Orange)
                _spriteRenderer.sprite = BlockSpriteCache.Instance.OrangeBlockSprite;
            else if (colorType == ColorType.Purple)
                _spriteRenderer.sprite = BlockSpriteCache.Instance.PurpleBlockSprite;



        }
    }
}