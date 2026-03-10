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

        protected override void InitComponents()
        {
            //Component Initialization
            _gridDataAccessor = ColorBlockGridDataAccessor.Instance;
            _levelSelectManager = LevelSelectManager.Instance;
            _colorGridManager = ColorGridManager.Instance;
        }

        protected override void InitContext()
        {
            _context.AddSingletonData(_gridDataAccessor);
            _context.AddSingletonData(_levelSelectManager);
            _context.AddSingletonData(_colorGridManager);
        }

        protected override void InitCORHandler()
        {
            _rootHandler = new SetupGridHandler();
        }
    }

    class SetupGridHandler : AbstractHandler
    {
        public override void Handle(CORContext request)
        {
            var gridDataAccessor = request.GetSingletonData<ColorBlockGridDataAccessor>();
            var levelSelectManager = request.GetSingletonData<LevelSelectManager>();
            var colorGridManager = request.GetSingletonData<ColorGridManager>();


            Level level = levelSelectManager.CurrentLevel;
            LevelConfigurationData levelConfigurationData = gridDataAccessor.GetColorBlockConfigurationDataByID(level.LevelID);
            level.SetLevelConfigurationData(levelConfigurationData);

            var puzzleGridConfig = level.GetGridConfiguration();

            colorGridManager.InitializeGrid(puzzleGridConfig);

            base.Handle(request);
        }
    }


}