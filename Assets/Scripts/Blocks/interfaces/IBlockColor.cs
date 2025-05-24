using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Blocks.interfaces
{
    public enum ColorRank { Primary, Secondary }
    public interface IBlockColor
    {

        public ColorRank GetColorRank();
        public Color GetColor();
        public bool CanCombine(IBlockColor other);
        public IBlockColor GetCombineColor(IBlockColor blockColor);

    }
}