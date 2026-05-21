using Assets.Scripts.Blocks.components;
using Assets.Scripts.Blocks.components.managers;
using Assets.Scripts.Data;
using Assets.Scripts.Grid.components;
using Assets.Scripts.Systems.Data;
using Assets.Scripts.Systems.Goal;
using Assets.Scripts.Systems.LevelSelect;
using Assets.Scripts.Tools.Logic;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace Assets.Scripts.Systems.SceneLoading.FreePlay
{
    internal class FreePlaySceneSetup : SceneSetupBase
    {

        private ColorBlockGridDataAccessor _gridDataAccessor;
        private LevelSelectManager _levelSelectManager;
        private ColorGridManager _colorGridManager;
        private BlockManager _blockManager;
        private GoalManager _goalManager;

        protected override void InitComponents()
        {
            //Component Initialization
            _gridDataAccessor = ColorBlockGridDataAccessor.Instance;
            _levelSelectManager = LevelSelectManager.Instance;
            _colorGridManager = ColorGridManager.Instance;
            _blockManager = BlockManager.Instance;
            _goalManager = GoalManager.Instance;

        }

        protected override void InitContext()
        {
            _context.AddSingletonData(_gridDataAccessor);
            _context.AddSingletonData(_levelSelectManager);
            _context.AddSingletonData(_colorGridManager);
            _context.AddSingletonData(_blockManager);
            _context.AddSingletonData(_goalManager);
        }

        protected override void InitCORHandler()
        {
            _rootHandler = new SetupLevelHandler();

            _rootHandler.SetNext(new SetupGridHandler())
                        .SetNext(new SetupQueueHandler())
                        .SetNext(new GameStartHandler());
        }
    }

    class SetupGridHandler : AbstractHandler
    {
        public override void Handle(CORContext context)
        {
            var colorGridManager = context.GetSingletonData<ColorGridManager>();

            var configData = new LevelConfigurationData() { id = "_freePlay", name = "Free Play", height = 20, width = 12};
            var gridConfig = new GridConfiguration(configData);

            colorGridManager.InitializeGrid(gridConfig);

            base.Handle(context);
        }
    }
    class SetupQueueHandler : AbstractHandler
    {
        public override void Handle(CORContext context)
        {

            var blockManager = context.GetSingletonData<BlockManager>();
            blockManager.SetSpawningStrategy(new FreePlaySpawningStrategy());
            base.Handle(context);
        }
    }
    class SetupLevelHandler : AbstractHandler
    {
        public override void Handle(CORContext context)
        {


            base.Handle(context);
        }
    }
    class GameStartHandler : AbstractHandler
    {
        public override void Handle(CORContext context)
        {
            var blockManager = context.GetSingletonData<BlockManager>();

            blockManager.TriggerBlockCreation();


            base.Handle(context);
        }
    }

}