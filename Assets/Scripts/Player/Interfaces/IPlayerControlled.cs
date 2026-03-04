using System;

namespace Assets.Scripts.Player.Interfaces
{
    public interface IPlayerControlled
    {

        public event Action OnPlayerControlCompleted;

        void SetEnabled(bool state);
    }
}