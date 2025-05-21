using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Blocks.components
{
    public class BlockFactory : MonoBehaviour
    {

        [SerializeField]
        private Transform blockPrefab;

        public ColorBlock CreateBlock(Color color)
        {
            ColorBlock block = Instantiate(blockPrefab, new Vector2(0,0), Quaternion.identity).GetComponent<ColorBlock>();
            block.SetColorRank(interfaces.ColorRank.Primary);
            return block;
        }
    }
}