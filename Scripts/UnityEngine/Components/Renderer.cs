using Godot;
using System;

namespace UnityEngine
{
    public partial class Renderer : Component, IHierarchyActiveDependent
    {
        public Material sharedMaterial;
        public Material material { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        [Export] public bool enabled { get; set; }
        public virtual void OnHierarchyActive(bool active) { }

    }
}
