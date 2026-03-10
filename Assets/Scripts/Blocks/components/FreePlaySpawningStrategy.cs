using Assets.Scripts.Blocks.components;
using Assets.Scripts.Blocks.components.colors;
using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.Data;
using Assets.Scripts.Systems.LevelSelect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class FreePlaySpawningStrategy : ISpawningStrategy
{


    void ISpawningStrategy.HandlePlayerControlCompleted(ISpawningStrategyListener listener)
    {
        listener.CreateNewBlock();
    }

    ITakeBlockCommand ISpawningStrategy.SpawnBlock(ISpawningStrategyListener listener, string blockID)
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
        listener.CreateNewBlock();
    }
}


public class PuzzleSpawningStrategy : ISpawningStrategy
{

    private Dictionary<string, ColorBlockGroupConfiguration> _configurationCache;
    private readonly LevelSelectManager _levelSelectManager;

    public PuzzleSpawningStrategy(LevelSelectManager levelSelectManager)
    {

        _configurationCache = new Dictionary<string, ColorBlockGroupConfiguration>();
        _levelSelectManager = levelSelectManager;


        ColorBlockConfigurationCache _colorBlockConfigruationCache = new ColorBlockConfigurationCache();
       

        if(_colorBlockConfigruationCache == null)
        {
            throw new Exception("Failed to initialize ColorBlockConfigurationCache.");
        }

        (_colorBlockConfigruationCache as IDataConfigurationCache).LoadFromDisk();

        foreach (var config in _colorBlockConfigruationCache.Configurations)
        {
            var key = config.id;
            var value = new ColorBlockGroupConfiguration(config);
            _configurationCache[key] = value;
        }

    }

    void ISpawningStrategy.HandlePlayerControlCompleted(ISpawningStrategyListener listener)
    {
        listener.CreateNewBlock();

    }
    ITakeBlockCommand ISpawningStrategy.SpawnBlock(ISpawningStrategyListener listener, string blockID)
    {

        try
        {
            var target = listener.BlockFactory.CreateBlockGroup();

            var blockConfiguration = _configurationCache[blockID];
            var blockDataSet = (blockConfiguration as IBlockGroupConfiguration).GetPositions();

            foreach (var blockData in blockDataSet)
            {
                var colorType = blockData.BlockColor;
                var gridPosition = blockData.Position;
                var block = listener.BlockFactory.CreateBlock(colorType) as IBlock;
                (target as IBlockGroup).AddBlock(block, gridPosition); //Add the block to the group

            }

            (target as ColorBlockGroupController).Initialize();
            return target;


        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to spawn block with ID {blockID}: {e.Message}");
            return null;
        }

    }
    void ISpawningStrategy.SpawningSetup(ISpawningStrategyListener listener)
    {

    }

}
