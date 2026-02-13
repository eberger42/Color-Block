using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.Grid.interfaces;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Grid.components
{

    public class NodeDataRemoved : NodeEvent
    {
        public INodeData RemovedData { get; private set; }
        public NodeDataRemoved(INode sender, INodeData removedData) : base(sender)
        {
            this.RemovedData = removedData;
        }
    }

    public class NodeDataSet : NodeEvent
    {
        public NodeDataSet(INode sender) : base(sender)
        {
        }
    }

    public class NodeDataColorChanged : NodeEvent
    {
        public INodeData NodeData { get; private set; }

        public NodeDataColorChanged(INode sender) : base(sender)
        {
        }
    }

    public class NodeDataLanded : NodeEvent
    {
        public INodeData NodeData { get; private set; }

        public NodeDataLanded(INode sender) : base(sender)
        {
        }
    }
    public abstract class NodeEvent : INodeEvent
    {
        private INode sender;

        public NodeEvent(INode sender)
        {
            this.sender = sender;
        }

        public virtual INode GetSender()
        {
            return sender;
        }
    }
}