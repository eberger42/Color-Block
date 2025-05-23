using Assets.Scripts.Blocks.scriptable_objects;
using Assets.Scripts.Grid.components;
using UnityEngine;

public interface INode
{ 
    public void SetPosition(GridPosition position);
    public GridPosition GetGridPosition();
    public void GenerateNode();
    public void Configure(NodeConfiguration config, GridPosition gridPosition);
    public void SetGridListener<T>(T gridListener);

    //Control
    public void SetNodeData<K>(K nodeData);
    public void ClearNodeData<K>(K nodeData);
    public INode GetNeighbor(GridPosition direction);

    //FeedBack
    public bool IsOccupied();
    public bool IsNeighborOccupied(GridPosition direction);

}
