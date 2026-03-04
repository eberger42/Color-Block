using Assets.Scripts.Blocks.components.colors;
using Assets.Scripts.Blocks.interfaces;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class FreePlaySpawningStrategy : ISpawningStrategy
{


    void ISpawningStrategy.HandlePlayerControlCompleted(ISpawningStrategyListener listener)
    {
        listener.CreateNewBlock();
    }

    ITakeBlockCommand ISpawningStrategy.SpawnBlock(ISpawningStrategyListener listener)
    {
        var blockColor = BlockColor.GenerateRandomPrimaryColor();
        var target = listener.BlockFactory.CreateBlockGroup(blockColor);

        return target;
    }

    void ISpawningStrategy.SpawningSetup(ISpawningStrategyListener listener)
    {
        (listener as MonoBehaviour).StartCoroutine(SpawnNextFrame(listener));
    }


    private IEnumerator SpawnNextFrame(ISpawningStrategyListener listener)
    {
        yield return null;
        (this as ISpawningStrategy).SpawnBlock(listener);
    }
}


public class PuzzleSpawningStrategy : ISpawningStrategy
{
    void ISpawningStrategy.HandlePlayerControlCompleted(ISpawningStrategyListener listener)
    {
        listener.CreateNewBlock();

    }
    ITakeBlockCommand ISpawningStrategy.SpawnBlock(ISpawningStrategyListener listener)
    {
        var blockColor = BlockColor.GenerateRandomPrimaryColor();
        var target = listener.BlockFactory.CreateBlockGroup(blockColor);
        return target;
    }
    void ISpawningStrategy.SpawningSetup(ISpawningStrategyListener listener)
    {
        (listener as MonoBehaviour).StartCoroutine(SpawnNextFrame(listener));
    }


    private IEnumerator SpawnNextFrame(ISpawningStrategyListener listener)
    {
        yield return null;
        listener.CreateNewBlock();
    }
}
