using Assets.Scripts.General.interfaces;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.General
{
    public class GameTickManager : MonoBehaviour, ITickManager
    {

        public static GameTickManager Instance { get; private set; }


        [SerializeField] private float gameTickInterval = 1.0f;
        private float currentTime = 0f;

        public event Action OnTick;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            Instance = null;
        }


        private void FixedUpdate()
        {
            currentTime += Time.fixedDeltaTime;

            if (currentTime >= gameTickInterval)
            {
                Debug.Log("Tick");
                OnGameTick();
                currentTime = 0f;
            }
        }

        private void OnGameTick()
        {
            OnTick?.Invoke();
        }



    }
}