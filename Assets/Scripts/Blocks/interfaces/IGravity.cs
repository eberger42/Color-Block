using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Blocks.interfaces
{
    public interface IGravity 
    {
        public event Action<bool> OnEnableGravity;

        void SetEnable(bool state);
        public bool CheckIfFloating();


    }
}