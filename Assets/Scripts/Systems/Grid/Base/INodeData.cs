using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Grid.interfaces
{
    public interface INodeData
    {
        public INode GetNodeDataParent();

        public void SetNodeDataParent(INode node);

        public void HandleNodeEvent(INodeEvent nodeEvent);
    }
}