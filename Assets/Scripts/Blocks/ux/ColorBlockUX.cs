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
        private Animator _colorChangeAnimation;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _colorBlock = GetComponent<ColorBlock>();
            _colorChangeAnimation = GetComponent<Animator>();

            (_colorBlock as IBlock).OnColorUpdated += UpdateColor;

        }

        private void UpdateColor(BlockColorUpdateEventArgs color)
        {

            var newColor = color.NewColor;    
            var incomingColor = color.IncomingColor;
            var incomingDirection = color.IncomingDirection;



            var newColorType = newColor.GetColorType();

            if(newColorType == ColorType.Red || newColorType == ColorType.Blue || newColorType == ColorType.Yellow)
            {
                _spriteRenderer.sprite = BlockSpriteCache.Instance.ColorBlockSprite;
                _spriteRenderer.material.SetColor("_BaseColor", incomingColor.Color);
                _spriteRenderer.material.SetColor("_FillColor", incomingColor.Color);

            }
            else if (newColorType == ColorType.Green || newColorType == ColorType.Orange || newColorType == ColorType.Purple)
            {
                _spriteRenderer.material.SetColor("_FillColor", newColor.Color);
                _spriteRenderer.material.SetVector("_Direction", incomingDirection.ToVector2());
                _colorChangeAnimation.SetTrigger("Fill");
            }

        }

    }
}