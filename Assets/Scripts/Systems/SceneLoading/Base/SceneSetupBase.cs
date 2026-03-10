
using Assets.Scripts.Tools.Logic;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Systems.SceneLoading
{

    /// <summary>
    /// Provides a base class for scene setup operations in Unity, enabling derived types to implement custom
    /// initialization logic using handlers.
    /// </summary>
    /// <remarks>This abstract class is intended to be inherited by scene setup components that require
    /// handler-based initialization. It manages the invocation of setup routines at the start of the scene lifecycle
    /// and provides a structure for handling requests via a root handler.</remarks>
    internal abstract class SceneSetupBase : MonoBehaviour
    {

        protected IHandler _rootHandler;
        protected CORContext _context = new CORContext();

        protected virtual void Start()
        {
            InitComponents();
            InitContext();
            InitCORHandler();
            StartCoroutine(DelayedSetup());
        }

        /// <summary>
        /// Called In the Start method to initialize necessary components before the setup process begins.
        /// </summary>
        protected abstract void InitComponents();

        protected abstract void InitCORHandler();
        protected abstract void InitContext();

        protected virtual void Setup()
        {
            if(_rootHandler == null)
            {
                Debug.LogWarning("Root handler is not set up. Please set it up in the Setup method.");
                return;
            }

            _rootHandler.Handle(_context);
        }

        IEnumerator DelayedSetup()
        {
            yield return null;
            Setup();
        }
    }
}

