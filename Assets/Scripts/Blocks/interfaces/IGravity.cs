using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Blocks.interfaces
{
    public interface IGravity 
    {
        public event Action<bool> OnEnableGravity;
        public event Action OnTriggerGravity;

        void SetEnable(bool state);
        public bool CheckIfFloating();
        public void Trigger();


    }
}