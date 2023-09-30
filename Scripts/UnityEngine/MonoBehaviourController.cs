using Godot;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine
{
    public class MonoBehaviourController
    {
        Node node;
        MonoBehaviour monoBehaviour;
        public List<IEnumerator> coroutines = new List<IEnumerator>();
        bool startCalled;

        public string name { get { return node.Name; } set { node.Name = (value); } }

        MethodInfo awakeMethod;
        MethodInfo startMethod;
        MethodInfo startMethodCR;
        MethodInfo updateMethod;
        MethodInfo fixedUpdateMethod;
        MethodInfo onEnabledMethod;
        MethodInfo onDisabledMethod;

        const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;



        bool previouslyEnabledAndInActiveHierarchy = true;


        MethodInfo FindMethod(string methodName, System.Type returnType, System.Type type = null)
        {
            type = type == null ? node.GetType() : type;
            MethodInfo method = type.GetMethod(methodName, bindingFlags);

            if (method != null)
            {
                if (method.GetParameters().Length == 0)
                {
                    if (method.ReturnType == returnType)
                    {
                        return method;
                    }
                }
            }

            return (type == typeof(MonoBehaviour) || type.BaseType == null) ? null : FindMethod(methodName, returnType, type.BaseType);
        }


        public void Initialize(MonoBehaviour _monoBehaviour, Node _node)
        {
            node = _node;
            monoBehaviour = _monoBehaviour;
            //if (referencedNode is Node3D)
            //{
            //    Node3D spatial = referencedNode as Node3D;
            //    visibilityHandler = new SpatialVisibilityHandler(spatial);
            //}
            //else if (referencedNode is CanvasItem)
            //{
            //    CanvasItem canvasItem = referencedNode as CanvasItem;
            //    visibilityHandler = new CanvasItemVisibilityHandler(canvasItem);
            //}
            //else
            //{
            //    visibilityHandler = new VisibilityHandler();
            //}

            awakeMethod = FindMethod("Awake", typeof(void));
            startMethod = FindMethod("Start", typeof(void));
            startMethodCR = FindMethod("Start", typeof(IEnumerator));
            updateMethod = FindMethod("Update", typeof(void));
            fixedUpdateMethod = FindMethod("FixedUpdate", typeof(void));
            onEnabledMethod = FindMethod("OnEnable", typeof(void));
            onDisabledMethod = FindMethod("OnDisable", typeof(void));

            if (awakeMethod != null)
            {
                awakeMethod.Invoke(this.node, (object[])null);
            }
        }


        //VisibilityHandler visibilityHandler;
        //bool prevVisibility;
        //void _on_visibility_changed()
        //{
        //    if (prevVisibility != visibilityHandler.IsVisible)
        //    {
        //        bool curVisibility = visibilityHandler.IsVisible;

        //        if (curVisibility && onEnabledMethod != null)
        //        {
        //            onEnabledMethod.Invoke(node, null);
        //        }
        //        else if (!curVisibility && onDisabledMethod != null)
        //        {
        //            onDisabledMethod.Invoke(node, null);
        //        }

        //        prevVisibility = curVisibility;
        //    }
        //}

        public void OnActiveChange(bool enabledAndInActiveHierarchy)
        {
            if (previouslyEnabledAndInActiveHierarchy == enabledAndInActiveHierarchy) return;


            if (enabledAndInActiveHierarchy && onEnabledMethod != null)
            {
                onEnabledMethod.Invoke(node, null);
            }
            else if (!enabledAndInActiveHierarchy && onDisabledMethod != null)
            {
                onDisabledMethod.Invoke(node, null);
            }

            previouslyEnabledAndInActiveHierarchy = enabledAndInActiveHierarchy;

        }


        public void Update()
        {
            try
            {
                //if (!visibilityHandler.IsVisible)
                //{
                //    return;
                //}

                if (!monoBehaviour.enabled) return;
                if (!monoBehaviour.gameObject.activeInHierarchy) return;
                if (!startCalled)
                {
                    startCalled = true;

                    if (startMethod != null)
                    {
                        startMethod.Invoke(node, null);
                    }
                    else if (startMethodCR != null)
                    {
                        StartCoroutine((IEnumerator)startMethodCR.Invoke(node, null));
                    }
                }

                if (updateMethod != null)
                {
                    updateMethod.Invoke(node, null);
                }

                for (int i = 0; i < coroutines.Count; i++)
                {
                    CustomYieldInstruction yielder = coroutines[i].Current as CustomYieldInstruction;
                    bool yielded = yielder != null && yielder.MoveNext();

                    if (!yielded && !coroutines[i].MoveNext())
                    {
                        coroutines.RemoveAt(i);
                        i--;
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message + "\n\n" + e.StackTrace);
            }
        }


        public void FixedUpdate()
        {
            if (fixedUpdateMethod != null)
            {
                fixedUpdateMethod.Invoke(node, null);
            }
        }


        public Coroutine StartCoroutine(IEnumerator routine)
        {
            coroutines.Add(routine);
            return new Coroutine(routine);
        }


        public static void Destroy(MonoBehaviour node)
        {
            node.QueueFree();
        }
    }
}