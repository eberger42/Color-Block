using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Blocks.interfaces
{
    public interface ISpawningStrategyListener
    {
        public IBlockFactory BlockFactory { get; }
    }
}
