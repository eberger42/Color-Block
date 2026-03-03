using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Blocks.components.colors
{
    public class BlockSpriteCache : MonoBehaviour
    {

        public static BlockSpriteCache Instance { get; private set; }


        public Sprite ColorBlockSprite { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }


            if (ColorBlockSprite == null)
                ColorBlockSprite = Resources.Load<Sprite>("Sprites/ColorBlock");

        }
        private void OnDestroy()
        {
            Instance = null;
        }

    }
}