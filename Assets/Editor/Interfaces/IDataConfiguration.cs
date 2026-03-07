using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Editor.Data
{
    public interface IDataConfiguration
    {
        string id { get; set; }
        string name { get; set; }
    }
}
