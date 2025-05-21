using Assets.Scripts.Blocks.scriptable_objects;
using Assets.Scripts.Grid.components;
using UnityEngine;

public interface INode
{ 
    public void SetPosition(GridPosition position);
    public GridPosition GetPosition();
    public void GenerateNode();
    public void Configure(NodeConfiguration config, GridPosition gridPosition);
    public void SetNodeData<K>(K nodeData);
    public void SetGridListener<T>(T gridListener);
    public abstract bool IsOccupied();

}
