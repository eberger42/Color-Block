using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Player.Interfaces
{
    public interface ICommand
    {
        public Task Execute();
    }
}