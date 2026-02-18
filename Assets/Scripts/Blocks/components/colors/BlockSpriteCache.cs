using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Blocks.components.colors
{
    public class BlockSpriteCache : MonoBehaviour
    {

        public static BlockSpriteCache Instance { get; private set; }


        public Sprite RedBlockSprite { get; private set; }
        public Sprite BlueBlockSprite { get; private set; }
        public Sprite YellowBlockSprite { get; private set; }
        public Sprite GreenBlockSprite { get; private set; }
        public Sprite OrangeBlockSprite { get; private set; }
        public Sprite PurpleBlockSprite { get; private set; }

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
                return;
            }


            if (RedBlockSprite == null)
                RedBlockSprite = Resources.Load<Sprite>("Sprites/RedBlock");
            if (BlueBlockSprite == null)
                BlueBlockSprite = Resources.Load<Sprite>("Sprites/BlueBlock");
            if (YellowBlockSprite == null)
                YellowBlockSprite = Resources.Load<Sprite>("Sprites/YellowBlock");
            if (GreenBlockSprite == null)
                GreenBlockSprite = Resources.Load<Sprite>("Sprites/GreenBlock");
            if (OrangeBlockSprite == null)
                OrangeBlockSprite = Resources.Load<Sprite>("Sprites/OrangeBlock");
            if (PurpleBlockSprite == null)
                PurpleBlockSprite = Resources.Load<Sprite>("Sprites/PurpleBlock");

        }
    }
}