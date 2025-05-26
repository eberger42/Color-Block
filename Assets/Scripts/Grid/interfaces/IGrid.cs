using Assets.Scripts.Blocks.scriptable_objects;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public struct GridPosition
{
    public int x;
    public int y;

    public GridPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static GridPosition Up => new GridPosition(0, 1);
    public static GridPosition Down => new GridPosition(0, -1);
    public static GridPosition Left => new GridPosition(-1, 0);
    public static GridPosition Right => new GridPosition(1, 0);

    public static List<GridPosition> Directions => new List<GridPosition>
    {
        Up,
        Down,
        Left,
        Right
    };


    public static GridPosition operator +(GridPosition a, GridPosition b) => new GridPosition(a.x + b.x, a.y + b.y);
    public static GridPosition operator -(GridPosition a, GridPosition b) => new GridPosition(a.x - b.x, a.y - b.y);
    public static GridPosition operator *(GridPosition a, GridPosition b) => new GridPosition(a.x * b.x, a.y * b.y);
    public static bool operator ==(GridPosition a, GridPosition b) => (a.x == b.x && a.y == b.y);
    public static bool operator !=(GridPosition a, GridPosition b) => (a.x != b.x && a.y != b.y);
    

    public override bool Equals(object obj)
    {
        return obj is GridPosition position &&
               x == position.x &&
               y == position.y;
    }

    public override int GetHashCode()
    {
        return (x, y).GetHashCode();
    }

    public override string ToString()
    {
        return $"[{x}, {y}]";
    }
}

/// <summary>
/// out T provides covariance, allowing the interface to be used with a more derived type.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IGrid<out T>
{

    public void GenerateGrid(NodeConfiguration config);

    public T GetNode(int x, int y);

    public bool IsInBounds(int x, int y);

    public bool IsSpacesOccupied(List<GridPosition> positions);
    
}
