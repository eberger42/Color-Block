using System.Collections;
using System.Windows.Input;
using UnityEngine;

namespace Assets.Scripts.Blocks.interfaces
{
    public interface IReceiveCommand
    {
        public void TakeCommand(ICommand command);
    }
}