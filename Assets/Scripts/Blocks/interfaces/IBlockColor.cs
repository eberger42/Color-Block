using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Blocks.interfaces
{
    public enum ColorRank { Primary, Secondary }
    public enum ColorType { Red, Blue, Yellow, Orange, Purple, Green }
    public interface IBlockColor
    {

        public ColorRank GetColorRank();
        public ColorType GetColorType();
        public bool CanCombine(IBlockColor other);
        public IBlockColor GetCombineColor(IBlockColor blockColor);

    }
}