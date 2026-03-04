using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Editor.Data
{
    public interface IDataConfigurationCache<T>
    {
            
            void UpdateConfiguration(T configuration);
            void SaveToDisk();
            void LoadFromDisk();
    }
}
