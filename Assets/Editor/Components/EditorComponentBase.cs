using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Editor.Components
{
    internal abstract class EditorComponentBase
    {
            public virtual void Refresh()
            {
            }
            public abstract void OnGUI();
            public abstract void OnEnable();
    }
}
