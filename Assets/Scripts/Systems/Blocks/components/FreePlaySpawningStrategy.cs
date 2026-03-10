using Assets.Scripts.Blocks.components;
using Assets.Scripts.Blocks.components.colors;
using Assets.Scripts.Blocks.components.managers;
using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.Data;
using Assets.Scripts.Systems.LevelSelect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class FreePlaySpawningStrategy : ISpawningStrategy
{


    void ISpawningStrategy.HandlePlayerControlCompleted(ISpawningStrategyListener listener)
    {
    }

    ITakeBlockCommand ISpawningStrategy.SpawnBlock(ISpawningStrategyListener listener)
    {
        //TODO: Implement a more complex spawning logic based on the blockID, for now we just spawn a random block

        return null;
    }

    void ISpawningStrategy.SpawningSetup(ISpawningStrategyListener listener)
    {
        (listener as MonoBehaviour).StartCoroutine(SpawnNextFrame(listener));
    }


    private IEnumerator SpawnNextFrame(ISpawningStrategyListener listener)
    {
        yield return null;
    }
}


public class PuzzleSpawningStrategy : ISpawningStrategy
{
    private PuzzleBlockQueue _puzzleBlockQueue;

    public PuzzleSpawningStrategy(PuzzleBlockQueue puzzleBlockQueue)
    {
        _puzzleBlockQueue = puzzleBlockQueue;
    }

    void ISpawningStrategy.HandlePlayerControlCompleted(ISpawningStrategyListener listener)
    {

    }
    ITakeBlockCommand ISpawningStrategy.SpawnBlock(ISpawningStrategyListener listener)
    {

        try
        {
            var target = listener.BlockFactory.CreateBlockGroup();

            var blockConfiguration = _puzzleBlockQueue.GetNextBlockGroup();
            var blockDataSet = (blockConfiguration as IBlockGroupConfiguration).GetPositions();

            foreach (var blockData in blockDataSet)
            {
                var block = listener.BlockFactory.CreateBlock(blockData.BlockColor) as IBlock;

                Debug.Log($"Spawning block of color {blockData.BlockColor} at position {blockData.Position}");
                (target as IBlockGroup).AddBlock(block, blockData.Position); //Add the block to the group

            }

            (target as ColorBlockGroupController).Initialize();
            return target;


        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to spawn block: {e.Message}");
            return null;
        }

    }
    void ISpawningStrategy.SpawningSetup(ISpawningStrategyListener listener)
    {

    }

}
