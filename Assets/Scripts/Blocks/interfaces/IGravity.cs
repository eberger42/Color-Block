using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Blocks.interfaces
{
    public interface IGravity 
    {

        public event Action OnBottomContact;
        void TriggerBottomReahed();

    }
}