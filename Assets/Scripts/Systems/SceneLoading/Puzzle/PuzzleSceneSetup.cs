using Assets.Scripts.Blocks.components;
using Assets.Scripts.Blocks.components.managers;
using Assets.Scripts.Data;
using Assets.Scripts.Grid.components;
using Assets.Scripts.Systems.Data;
using Assets.Scripts.Systems.LevelSelect;
using Assets.Scripts.Tools.Logic;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace Assets.Scripts.Systems.SceneLoading.Puzzle
{
    internal class PuzzleSceneSetup : SceneSetupBase
    {

        private ColorBlockGridDataAccessor _gridDataAccessor;
        private LevelSelectManager _levelSelectManager;
        private ColorGridManager _colorGridManager;
        private BlockManager _blockManager;

        protected override void InitComponents()
        {
            //Component Initialization
            _gridDataAccessor = ColorBlockGridDataAccessor.Instance;
            _levelSelectManager = LevelSelectManager.Instance;
            _colorGridManager = ColorGridManager.Instance;
            _blockManager = BlockManager.Instance;

        }

        protected override void InitContext()
        {
            _context.AddSingletonData(_gridDataAccessor);
            _context.AddSingletonData(_levelSelectManager);
            _context.AddSingletonData(_colorGridManager);
            _context.AddSingletonData(_blockManager);
        }

        protected override void InitCORHandler()
        {
            _rootHandler = new SetupLevelHandler();

            _rootHandler.SetNext(new SetupGridHandler())
                        .SetNext(new SetupQueueHandler());
        }
    }

    class SetupGridHandler : AbstractHandler
    {
        public override void Handle(CORContext context)
        {
            var colorGridManager = context.GetSingletonData<ColorGridManager>();
            var level = context.GetSingletonData<Level>();
            var puzzleGridConfig = new PuzzleGridConfiguration(level.LevelConfigData);

            colorGridManager.InitializeGrid(puzzleGridConfig);

            base.Handle(context);
        }
    }
    class SetupQueueHandler : AbstractHandler
    {
        public override void Handle(CORContext context)
        {

            var blockManager = context.GetSingletonData<BlockManager>();
            var level = context.GetSingletonData<Level>();

            var configList = new List<ColorBlockGroupConfiguration>();

            foreach (var blockID in level.LevelConfigData.queue)
            {
                var blockGroupConfig = blockManager.ConfigurationCache[blockID];
                configList.Add(blockGroupConfig);
            }

            var puzzleQueue = new PuzzleBlockQueue(configList);
            blockManager.SetSpawningStrategy(new PuzzleSpawningStrategy(puzzleQueue));

            base.Handle(context);
        }
    }

    class SetupLevelHandler : AbstractHandler
    {
        public override void Handle(CORContext context)
        {
            var gridDataAccessor = context.GetSingletonData<ColorBlockGridDataAccessor>();
            var levelSelectManager = context.GetSingletonData<LevelSelectManager>();
            var colorGridManager = context.GetSingletonData<ColorGridManager>();


            Level level = levelSelectManager.CurrentLevel;
            LevelConfigurationData levelConfigurationData = gridDataAccessor.GetColorBlockConfigurationDataByID(level.LevelID);
            level.SetLevelConfigurationData(levelConfigurationData);

            context.AddSingletonData(level);

            base.Handle(context);
        }
    }


}