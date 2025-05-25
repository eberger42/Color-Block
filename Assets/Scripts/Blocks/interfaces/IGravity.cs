using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Blocks.interfaces
{
    public interface IGravity 
    {

        public event Action OnBottomContact;
        public event Action OnNeedGravity;
        void TriggerBottomReahed();

        public bool CheckIfFloating();

    }
}