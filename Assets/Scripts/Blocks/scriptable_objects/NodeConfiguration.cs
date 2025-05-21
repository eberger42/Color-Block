using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Blocks.scriptable_objects
{
    [CreateAssetMenu(fileName = "NodeConfiguration", menuName = "ScriptableObjects/NodeConfiguration", order = 1)]
    public class NodeConfiguration : ScriptableObject
    {
        [SerializeField]
        private float size;
        [SerializeField]
        private Vector2 origin;
        [SerializeField] 
        private Texture2D texture;


        [SerializeField]
        private Color color;

        public float Size { get => size; }
        public Color Color { get => color; }
        public Vector2 Origin { get => origin; }
        public Texture2D Texture { get => texture; }
    }
}