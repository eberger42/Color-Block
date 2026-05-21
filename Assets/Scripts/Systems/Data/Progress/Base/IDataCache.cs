using Assets.Scripts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Systems.Data.Progress.Base
{
    public interface IDataCache
    {
        void Delete(string id);
        void SaveToDisk();
        void LoadFromDisk();
        IDataConfiguration GetConfigurationDataByID(string id);

    }
}
