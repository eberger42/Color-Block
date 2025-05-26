using Assets.Scripts.Grid.interfaces;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Grid.components
{
    public abstract class NodeData : INodeData
    {
        protected INode nodeParent;

        public virtual INode GetNodeDataParent()
        {
            return nodeParent;
        }


        public virtual void SetNodeDataParent(INode node)
        {
            this.nodeParent = node;
        }

        public abstract void HandleNodeEvent(INodeEvent nodeEvent);
    }

}