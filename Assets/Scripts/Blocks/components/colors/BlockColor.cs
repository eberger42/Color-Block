using Assets.Scripts.Blocks.interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Blocks.components.colors
{
    //OPTIMIZATION: Inherited colors can be made singletons and refrences as one, they are not going to change when on different blocks.
    public class BlockColor : IBlockColor
    {

        private static ColorConfiguration BuildRedConfig()
        {
            return new ColorConfiguration.Builder(ColorRank.Primary, ColorType.Red)
                .AddCombination(ColorType.Yellow, ColorType.Orange)
                .AddCombination(ColorType.Blue, ColorType.Purple)
                .Build();
        }

        private static ColorConfiguration BuildBlueConfig()
        {
            return new ColorConfiguration.Builder(ColorRank.Primary, ColorType.Blue)
                .AddCombination(ColorType.Yellow, ColorType.Green)
                .AddCombination(ColorType.Red, ColorType.Purple)
                .Build();
        }

        private static ColorConfiguration BuildYellowConfig()
        {
            return new ColorConfiguration.Builder(ColorRank.Primary, ColorType.Yellow)
                .AddCombination(ColorType.Red, ColorType.Orange)
                .AddCombination(ColorType.Blue, ColorType.Green)
                .Build();
        }

        private static ColorConfiguration BuildOrangeConfig()
        {
            return new ColorConfiguration.Builder(ColorRank.Secondary, ColorType.Orange)
                .Build();
        }

        private static ColorConfiguration BuildPurpleConfig()
        {
            return new ColorConfiguration.Builder(ColorRank.Secondary, ColorType.Purple)
                .Build();
        }
        
        private static ColorConfiguration BuildGreenConfig()
        {
            return new ColorConfiguration.Builder(ColorRank.Secondary, ColorType.Green)
                .Build();
        }


        public static List<BlockColor> PrimaryColors { get; } = new List<BlockColor>();
        public static List<BlockColor> SecondaryColors { get; } = new List<BlockColor>();

        public static BlockColor Red { get; } = new BlockColor(BuildRedConfig());
        public static BlockColor Blue { get; } = new BlockColor(BuildBlueConfig());
        public static BlockColor Yellow { get; } = new BlockColor(BuildYellowConfig());
        public static BlockColor Orange { get; } = new BlockColor(BuildOrangeConfig());
        public static BlockColor Purple { get; } = new BlockColor(BuildPurpleConfig());
        public static BlockColor Green { get; } = new BlockColor(BuildGreenConfig());



        public static IBlockColor GenerateRandomPrimaryColor()
        {
            var random = new System.Random();
            var randomColor = PrimaryColors[random.Next(PrimaryColors.Count)];
            return randomColor;
        }





        //Color Class Fields
        protected ColorRank _colorRank = ColorRank.Primary;
        protected ColorType _colorType;
        protected Dictionary<ColorType, ColorType> _combinations = new Dictionary<ColorType, ColorType>();


        private BlockColor(ColorConfiguration config)
        {
            this._colorRank = config.Rank;
            this._colorType = config.Type;
            this._combinations = config.Combinations;

            if(config.Rank == ColorRank.Primary)
            {
                PrimaryColors.Add(this);
            }
            else if(config.Rank == ColorRank.Secondary)
            {
                SecondaryColors.Add(this);
            }
        }


        ColorRank IBlockColor.GetColorRank()
        {
            return _colorRank;
        }
        ColorType IBlockColor.GetColorType()
        {
            return _colorType;
        }

        public bool CanCombine(IBlockColor other)
        {

            if (_colorRank == ColorRank.Secondary)
                return false;

            var colorType = other.GetColorType();

            if(_combinations.ContainsKey(colorType))
            {
                return true;
            }
            return false;
        }

        public IBlockColor GetCombineColor(IBlockColor blockColor)
        {
            return _combinations[blockColor.GetColorType()] switch
            {
                ColorType.Orange => Orange,
                ColorType.Purple => Purple,
                ColorType.Green => Green,
                _ => null
            };
        }



        private class ColorConfiguration
        {
            public ColorRank Rank { get; set; }
            public ColorType Type { get; set; }

            public Dictionary<ColorType, ColorType> Combinations { get; private set; }

            public ColorConfiguration(ColorRank rank, ColorType type)
            {
                Rank = rank;
                Type = type;
            }


            public class Builder
            {
                private ColorConfiguration _config;

                public Builder(ColorRank rank, ColorType type)
                {
                    _config = new ColorConfiguration(rank, type);
                }

                public Builder AddCombination(ColorType otherColor, ColorType resultColor)
                {
                    if (_config.Combinations == null)
                    {
                        _config.Combinations = new Dictionary<ColorType, ColorType>();
                    }
                    _config.Combinations[otherColor] = resultColor;
                    return this;
                }

                public ColorConfiguration Build()
                {
                    return _config;
                }
            }
        }
    }
}