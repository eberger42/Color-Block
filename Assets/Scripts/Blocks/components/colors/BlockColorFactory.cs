using Assets.Scripts.Blocks.interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Blocks.components.colors
{
    public static class BlockColorFactory
    {

        public static Func<IBlockColor> RedFactory = () => new RedBlockColor();
        public static Func<IBlockColor> YellowFactory = () => new YellowBlockColor();
        public static Func<IBlockColor> BlueFactory = () => new BlueBlockColor();

        public static IBlockColor CreateBlockColor(Color color)
        {
            IBlockColor blockColor = null;

            switch (color)
            {
                case Color red when red == Color.red:
                    blockColor = RedFactory();
                    break;
                case Color yellow when yellow == Color.yellow:
                    blockColor = YellowFactory();
                    break;
                case Color blue when blue == Color.blue:
                    blockColor = BlueFactory();
                    break;
                default:
                    throw new ArgumentException("Invalid color");
            }

            return blockColor;
        }

        public static IBlockColor GenerateRandomPrimaryColor()
        {
            int randomIndex = UnityEngine.Random.Range(0, 3);
            switch (randomIndex)
            {
                case 0:
                    return RedFactory();
                case 1:
                    return YellowFactory();
                case 2:
                    return BlueFactory();
                default:
                    throw new ArgumentOutOfRangeException("Invalid random index");
            }
        }
    }
}