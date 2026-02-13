using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.General.interfaces
{
    public interface ITickManager
    {
        public event Action OnTick;
    }
}