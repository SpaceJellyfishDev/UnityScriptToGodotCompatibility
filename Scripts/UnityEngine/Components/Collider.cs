using Godot;
using System;

namespace UnityEngine
{
    public partial class Collider : Component, IHierarchyActiveDependent
    {
        public Collider() => throw new NotImplementedException();

        [Export] public bool enabled { get; set; }
        public virtual void OnHierarchyActive(bool active) { }
    }
}
