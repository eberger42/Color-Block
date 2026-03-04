using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Blocks.interfaces
{
    internal interface ISpawningStrategy
    {
        void HandlePlayerControlCompleted(ISpawningStrategyListener listener);
        void SpawnBlock(ISpawningStrategyListener listener);
    }
}
