using Assets.Scripts.Blocks.commands;
using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.Grid.components;
using Assets.Scripts.Player.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Blocks.components
{

    [RequireComponent(typeof(ColorBlock))]
    public class BlockCombining : MonoBehaviour
    {

        private ColorBlock _block;

        private void Awake()
        {
            _block = GetComponent<ColorBlock>();


        }

        private void OnDisable()
        {
        }

        private void CheckForCombination()
        {
                
        }



    }
}