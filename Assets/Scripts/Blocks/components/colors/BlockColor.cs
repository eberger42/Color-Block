using Assets.Scripts.Blocks.interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Blocks.components.colors
{
    public abstract class BlockColor : IBlockColor
    {
        protected ColorRank _colorRank = ColorRank.Primary;
        protected Color _color;


        public BlockColor(Color color, ColorRank rank)
        {
            this._color = color;
            this._colorRank = rank;
        }

        Color IBlockColor.GetColor()
        {
            return _color;
        }

        ColorRank IBlockColor.GetColorRank()
        {
            return _colorRank;
        }

        public abstract bool CanCombine(IBlockColor other);
        public abstract IBlockColor GetCombineColor(IBlockColor blockColor);


    }
}