
using Assets.Scripts.Data;
using UnityEngine;

namespace Assets.Scripts.Blocks.components.managers
{
    public class PuzzleBlockQueueManager : MonoBehaviour
    {

        private ColorBlockGridConfigruationCache _colorBlockGridConfigruationCache;

        private void Awake()
        {
            _colorBlockGridConfigruationCache = new ColorBlockGridConfigruationCache();
            (_colorBlockGridConfigruationCache as IDataConfigurationCache).LoadFromDisk();
        }


    }
}
