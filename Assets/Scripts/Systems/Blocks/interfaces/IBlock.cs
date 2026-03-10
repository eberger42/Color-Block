using Assets.Scripts.Blocks.components.colors;
using Assets.Scripts.General.interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Blocks.interfaces
{
    public interface IBlock : IEntity, ITick
    {

        public event Action<BlockColorUpdateEventArgs> OnColorUpdated;
        public event Action OnBlockRemoved;

        public IBlockColor CurrentColor { get; }

        public GridPosition GetGridPosition();
        public void SetColor(IBlockColor color);
        public void MergeColor(IBlockColor color, GridPosition direction);
        public void SetParent(IBlockGroup parent);
        public IBlockGroup GetParent();
        public bool DoColorsMatch(IBlock block);
        public bool CheckMergeCompatability(IBlock block);

        public void Remove();
    }
}