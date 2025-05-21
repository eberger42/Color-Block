using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Blocks.interfaces
{
    public enum ColorRank { Primary, Secondary }
    public interface IColor
    {

        public ColorRank GetColorRank();

        public void SetColorRank(ColorRank rank);

    }
}