using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Editor.Components
{
    internal abstract class EditorComponentBase
    {

        protected List<Func<bool>> IsActiveConditions { get; } = new List<Func<bool>>()
        {
                () => true
        };
        protected bool IsActive => IsActiveConditions.All(condition => condition());

        public void AddCondition(Func<bool> condition)
        {
            IsActiveConditions.Add(condition);
        }

        public virtual void Refresh()
        {
        }
        public abstract void OnGUI();
        public abstract void OnEnable();
    }
}
