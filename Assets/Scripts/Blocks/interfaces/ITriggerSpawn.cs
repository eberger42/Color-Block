using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Blocks.interfaces
{
    public interface ITriggerSpawn
    {
        public event Action OnTriggerSpawn;

        void SetEnabled(bool state);
    }
}