using System.Collections;
using System.Collections.Generic;
using Godot;

namespace UnityEngine
{
    //[UseAsMonoBehaviour]
    public partial class MonoBehaviour : Component, IHierarchyActiveDependent
    {
        public Coroutine StartCoroutine(IEnumerator routine)
        {
            MonoBehaviourController controller = UnityEngineAutoLoad.Instance.GetMonoBehaviourController(this);

            if (controller != null)
            {
                controller.coroutines.Add(routine);
            }

            return new Coroutine(routine);
        }


        ////     Enabled Behaviours are Updated, disabled Behaviours are not.
        ////[RequiredByNativeCode]
        [Export] public bool enabled { get; set; } = true;
        ////     Has the Behaviour had active and enabled called?
        public bool isActiveAndEnabled { get; }

        public virtual void OnHierarchyActive(bool active)
        {
            UnityEngineAutoLoad.Instance.GetMonoBehaviourController(this).OnActiveChange(enabled && active);
        }

        // Probably use an interface as the type contraint
        public static void DontDestroyOnLoad(object obj)
        {
            throw new System.NotImplementedException();
        }

        // Probably use an interface as the type contraint
        public static void Destroy(object obj)
        {
            throw new System.NotImplementedException();
        }
    }
}
