using Assets.Scripts.Blocks.scriptable_objects;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Grid.components
{
    public abstract class Node<T> : INode
    {

        protected T _data;

        public abstract void Configure(NodeConfiguration config, GridPosition gridPosition);

        public abstract void GenerateNode();

        public abstract GridPosition GetPosition();

        public abstract void SetNodeData<K>(K nodeData);

        public abstract void SetPosition(GridPosition position);

        public abstract bool IsOccupied();

        public abstract  void SetGridListener<K>(K gridListener);
    }
}