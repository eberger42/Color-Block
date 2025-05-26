using Assets.Scripts.Blocks.scriptable_objects;
using Assets.Scripts.Grid.components;
using Assets.Scripts.Grid.interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;

public interface INode
{ 

    //Events
    public event Action<INodeEvent> OnNodeEvent;

    //Setup
    public void Configure(NodeConfiguration config, GridPosition gridPosition);
    public void GenerateNode();

    public GridPosition GetGridPosition();
    public void SetGridListener(IGrid<INode> gridListener);

    //Control
    public void SetNodeData(INodeData nodeData);
    public void ClearNodeData(INodeData nodeData);
    public INode GetNeighbor(GridPosition direction);
    public List<INode> GetNeighbors();
    public INode GetRotationNode(GridPosition direction);

    //FeedBack
    public bool IsOccupied();
    public bool IsNeighborOccupied(GridPosition direction);


    //Event Handlers
    public void OnNodeDataEvent(INodeEvent nodeEvent);

}
