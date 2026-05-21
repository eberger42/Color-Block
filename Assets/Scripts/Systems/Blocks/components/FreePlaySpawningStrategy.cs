using Assets.Scripts.Blocks.components;
using Assets.Scripts.Blocks.components.colors;
using Assets.Scripts.Blocks.components.managers;
using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.Data;
using Assets.Scripts.Player.Interfaces;
using Assets.Scripts.Systems.LevelSelect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

[Serializable]
public class FreePlaySpawningStrategy : ISpawningStrategy
{


    private ColorType _lastColor;


    ///////////////////////////////////////////////////////////
    /// ISpawningStrategy implementation
    ///////////////////////////////////////////////////////////
    #region ISpawningStrategy implementation

    void ISpawningStrategy.HandlePlayerControlCompleted(ISpawningStrategyListener listener)
    {
        (listener as BlockManager).TriggerBlockCreation();
    }

    ITakeBlockCommand ISpawningStrategy.SpawnBlock(ISpawningStrategyListener listener)
    {
        //TODO: Implement a more complex spawning logic based on the blockID, for now we just spawn a random block

        if(listener is BlockManager blockManager)
        {
            ColorBlockGroupConfiguration blockConfiguration = blockManager.ConfigurationCache.ElementAt(UnityEngine.Random.Range(0, blockManager.ConfigurationCache.Count)).Value;
            ITakeBlockCommand blockGroup = blockManager.BlockFactory.CreateBlockGroup();
            (blockGroup as ColorBlockGroupController).Initialize((blockConfiguration as IBlockGroupConfiguration).GetPivotPosition());

            var blockDataSet = (blockConfiguration as IBlockGroupConfiguration).GetPositions();
            var randomColor = GenerateRandomPrimaryColor();

            _lastColor = (randomColor as IBlockColor).GetColorType();

            foreach (var blockData in blockDataSet)
            {

                var block = listener.BlockFactory.CreateBlock((blockData.BlockColor as IBlockColor).GetColorType() == ColorType.White ? randomColor : (blockData.BlockColor)) as IBlock;
                (blockGroup as IBlockGroup).AddBlock(block, blockData.Position); //Add the block to the group

            }

            return blockGroup;
        }

        return null;
    }

    void ISpawningStrategy.SpawningSetup(ISpawningStrategyListener listener)
    {
        (listener as MonoBehaviour).StartCoroutine(SpawnNextFrame(listener));
    }

    #endregion

    //////////////////////////////////////////////////////////
    /// Private methods
    //////////////////////////////////////////////////////////
    #region Private methods
    private IEnumerator SpawnNextFrame(ISpawningStrategyListener listener)
    {
        yield return null;
    }

    private BlockColor GenerateRandomPrimaryColor()
    { //TODO: Potentially update so it is sudo random and not truly random to ensure a better distribution of colors
        var primaryColors = BlockColor.PrimaryColors.Where(x => ((x as IBlockColor).GetColorType()) != _lastColor).ToList();
        return primaryColors[UnityEngine.Random.Range(0, primaryColors.Count)];
    }
    #endregion

    ////////////////////////////////////////////////////////////
    /// ITakeFire1Input implementation
    //////////////////////////////////////////////////////////
    #region ITakeFire1Input

    void ITakeFire1Input.HandleFire1Logic(object listener)
    {

    }

    #endregion
}


public class PuzzleSpawningStrategy : ISpawningStrategy
{
    private PuzzleBlockQueue _puzzleBlockQueue;

    public PuzzleSpawningStrategy(PuzzleBlockQueue puzzleBlockQueue)
    {
        _puzzleBlockQueue = puzzleBlockQueue;
    }
    ///////////////////////////////////////////////////////////
    /// ISpawningStrategy implementation
    ///////////////////////////////////////////////////////////
    #region ISpawningStrategy implementation

    void ISpawningStrategy.HandlePlayerControlCompleted(ISpawningStrategyListener listener)
    {

    }
    ITakeBlockCommand ISpawningStrategy.SpawnBlock(ISpawningStrategyListener listener)
    {

        try
        {
            var target = listener.BlockFactory.CreateBlockGroup();

            var blockConfiguration = _puzzleBlockQueue.GetNextBlockGroup();

            if(blockConfiguration == null)
            {
                throw new Exception("No more block groups available in the puzzle queue");
            }


            var blockDataSet = (blockConfiguration as IBlockGroupConfiguration).GetPositions();

            foreach (var blockData in blockDataSet)
            {
                var block = listener.BlockFactory.CreateBlock(blockData.BlockColor) as IBlock;

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
    #endregion

    ////////////////////////////////////////////////////////////
    /// ITakeFire1Input implementation
    //////////////////////////////////////////////////////////
    #region ITakeFire1Input

    void ITakeFire1Input.HandleFire1Logic(object listener)
    {

        if(listener is BlockManager blockManager)
        {
            blockManager.TriggerBlockCreation();
        }
        else
        {
            Debug.LogError("Listener is not of type BlockManager");
        }

    }

    #endregion

}
