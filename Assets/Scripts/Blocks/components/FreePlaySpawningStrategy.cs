using Assets.Scripts.Blocks.components;
using Assets.Scripts.Blocks.components.colors;
using Assets.Scripts.Blocks.interfaces;
using UnityEngine;

public class FreePlaySpawningStrategy : ISpawningStrategy
{


    void ISpawningStrategy.HandlePlayerControlCompleted(ISpawningStrategyListener listener)
    {
        listener.CreateNewBlock();
    }

    void ISpawningStrategy.SpawnBlock(ISpawningStrategyListener listener)
    {
    }
}
