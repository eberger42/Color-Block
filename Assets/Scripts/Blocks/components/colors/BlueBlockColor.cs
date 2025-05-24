using Assets.Scripts.Blocks.interfaces;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Blocks.components.colors
{
    public class BlueBlockColor : BlockColor
    {
        public BlueBlockColor() : base(Color.blue, ColorRank.Primary)
        {
        }

        public override bool CanCombine(IBlockColor other)
        {
            throw new System.NotImplementedException();
        }

        public override IBlockColor GetCombineColor(IBlockColor blockColor)
        {
            throw new System.NotImplementedException();
        }
    }
}