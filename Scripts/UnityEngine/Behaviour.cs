using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Scripting;

namespace UnityEngine
{
    public partial class Behaviour : Component, IHierarchyActiveDependent
    {
        ////     Enabled Behaviours are Updated, disabled Behaviours are not.
        ////[RequiredByNativeCode]
        [Export] public bool enabled { get; set; }
        ////     Has the Behaviour had active and enabled called?
        public bool isActiveAndEnabled { get; }

        public virtual void OnHierarchyActive(bool active) { }
    }
}
