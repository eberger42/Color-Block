using Assets.Scripts.General.interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Blocks.interfaces
{
    public interface IBlock : IEntity, ITick
    {

        public event Action<IBlockColor> OnColorUpdated;

        public IBlockColor Color { get; }

        public GridPosition GetGridPosition();
        public void SetColor(IBlockColor color);
        public void SetParent(IBlockGroup parent);
        public IBlockGroup GetParent();
        public bool DoColorsMatch(IBlock block);
        public bool CheckMergeCompatability(IBlock block);
    }
}