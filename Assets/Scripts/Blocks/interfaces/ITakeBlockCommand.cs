using Assets.Scripts.Blocks.commands;
using Assets.Scripts.Blocks.components;
using Assets.Scripts.Grid.components;
using Assets.Scripts.Player.Interfaces;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Blocks.interfaces
{
    public interface ITakeBlockCommand
    {
        //Commands
        public void Move(GridPosition direction);
        public void Gravity();
        public void Rotate(GridPosition delta);
        public void Place(Grid<BlockNode> colorGrid, GridPosition position);

        //Checks
        public bool CheckForValidMove(GridPosition direction);
        public bool CheckForValidRotation(GridPosition position);
        public bool CanTakeCommand(ICommand command);
        public bool CanTakeCommand(Type commandType);

        //Helpers
        public void AddActionCommand(Func<Task> action);
        public void AddCommandToFilter(BlockCommand blockCommand);
        public void RemoveCommandFromFilter(BlockCommand blockCommand);
        public void AddCommandToFilter(Type blockCommandType);
        public void RemoveCommandFromFilter(Type blockCommandType);
    }
}