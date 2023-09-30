using Godot;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace UnityEngine
{
    public partial class Component : Node
    {

        public Transform transform { get { return gameObject.transform; } }

        public GameObject gameObject { get; private set; }

        internal void setGO(GameObject _gameObject) { gameObject = _gameObject; }

       


        public static void Destroy(Node node)
        {
            node.Free();
        }


        public T AddComponent<T>() where T : Component, new() => gameObject.AddComponent<T>();



        public T GetComponent<T>() where T : Component
        {
            if (this is T)
            {
                return this as T;
            }
            return gameObject.GetComponent<T>();

        }


        public T GetComponentInParent<T>() where T : Component=>gameObject.GetComponentInParent<T>();



        public T[] GetComponentsInParent<T>() where T : Component => gameObject.GetComponentsInParent<T>();



        public T GetComponentInChildren<T>() where T : Component => gameObject.GetComponentInChildren<T>(); 


        public T[] GetComponentsInChildren<T>() where T : Component => gameObject.GetComponentsInChildren<T>(); 



        //utilities
        protected static T GetNodeInParent<T>(Node node) where T : Node
        {
            Node curNode = node.GetParent();

            do
            {
                if (curNode is T)
                {
                    return curNode as T;
                }

                curNode = curNode.GetParent();
            }
            while (curNode != null);

            return null;
        }


    }
}
