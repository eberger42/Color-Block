using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.General
{
    public class GameTickManager : MonoBehaviour
    {
        public static GameTickManager Instance { get; private set; }

        public event Action OnTick;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private float gameTickInterval = 1.0f;
        private float currentTime = 0f;

        private void FixedUpdate()
        {
            currentTime += Time.fixedDeltaTime;

            if (currentTime >= gameTickInterval)
            {
                OnGameTick();
                currentTime = 0f;
            }
        }

        private void OnGameTick()
        {
            // Implement the logic that should happen on each game tick
            Debug.Log("Game tick occurred at: " + Time.time);
            OnTick?.Invoke();
        }



    }
}