using Assets.Scripts.Player.Interfaces;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Blocks.interfaces
{
    public interface IConfigureCommand
    {
        void Configure(ICommand command);
    }
}