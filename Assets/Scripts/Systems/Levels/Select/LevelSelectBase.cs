
using UnityEngine;

namespace Assets.Scripts.Systems.LevelSelect
{

    public abstract class LevelSelectBase : MonoBehaviour, ILevelSelect
    {

        [SerializeField]
        protected Transform _buttonContainer;

        [SerializeField]
        protected Transform _buttonPrefab;

        [SerializeField]
        private SceneController sceneController;

        protected virtual void Start()
        {
            LoadLevelData();
        }

        public abstract void LoadLevelData();

    }
    public interface ILevelSelect
    {
        void LoadLevelData();
    }
}

