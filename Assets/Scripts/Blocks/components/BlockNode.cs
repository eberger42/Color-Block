

using Assets.Scripts.Blocks.commands;
using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.Blocks.scriptable_objects;
using Assets.Scripts.Grid.components;
using Assets.Scripts.Grid.interfaces;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

namespace Assets.Scripts.Blocks.components
{
    public class BlockNode : Node
    {

        public BlockNode()
        {
            
        }

        public override void OnNodeDataEvent(INodeEvent nodeEvent)
        {
            if(_data is INodeData)
            {
                _data.HandleNodeEvent(nodeEvent);
            }
        }

        public override void SetNodeData(INodeData nodeData) 
        {

            if (nodeData is INodeData block)
            {
                this._data = block;
                if ((this._data is MonoBehaviour))
                    this._data.SetNodeDataParent(this);

                this.TriggerNodeEvent(new NodeDataSet(this));
            }
            else
            {
                Debug.LogError($"Node data is not of type {typeof(IBlock)}");
            }
        }

    }
}