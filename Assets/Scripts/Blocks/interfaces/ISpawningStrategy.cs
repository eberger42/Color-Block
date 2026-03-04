using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Blocks.interfaces
{
    internal interface ISpawningStrategy
    {

        void SpawningSetup(ISpawningStrategyListener listener);
        void HandlePlayerControlCompleted(ISpawningStrategyListener listener);
        ITakeBlockCommand SpawnBlock(ISpawningStrategyListener listener);
    }
}
