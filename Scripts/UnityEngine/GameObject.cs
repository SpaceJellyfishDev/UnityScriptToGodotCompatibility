using System;
using Godot;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	//[Tool]
	public partial class GameObject : Node
	{
		//Node node;
		public string name { get { return Name; } set { Name = (value); } }

		[Export] bool active { get { return activeSelf; } set { SetActive(value); } }
		//[Export] bool ready { get => false; set { _Ready(); } }
		//Transform _transform;
		public Transform transform //{ get { if (_transform == null) _transform = GetComponent<Transform>(); return _transform; } }
		{ get; private set; }
		[Export]
		public int layer { get; set; }


		//public GameObject()
		//{
		//    name = "Game Object";
		//    UnityEngineAutoLoad.Instance.AddChild(this);
		//}


		//public GameObject(string name)
		//{
		//    this.name = name;
		//    UnityEngineAutoLoad.Instance.AddChild(this);
		//}

		//public GameObject(Node _node)
		//{
		//    node = _node;
		//   // UnityEngineAutoLoad.Instance.AddChild(this);
		//}

		public bool activeSelf { get; private set; } = true;
		bool hierarchyActive = true;
		public bool activeInHierarchy { get => activeSelf && hierarchyActive; }
		internal void setHierarchyActive(bool active)
		{
			hierarchyActive = active;
			PropertyInfo visibleProperty = this.GetType().GetProperty("Visible");

			if (visibleProperty != null)
			{
				visibleProperty.SetValue(this, activeInHierarchy);
			}

			for (int i = hierarchyComponentList.Count - 1; i >= 0; i--)
			{
				if (hierarchyComponentList[i] == null) { hierarchyComponentList.RemoveAt(i); continue; }
				hierarchyComponentList[i].OnHierarchyActive(active);
			}
		}
		public void SetActive(bool active)
		{

			activeSelf = active;

			List<GameObject> childGOList = new List<GameObject>();
			CollectChildNode<GameObject>(this, childGOList);
			foreach (GameObject go in childGOList)
			{
				go.setHierarchyActive(active);
			}
		}


		List<IHierarchyActiveDependent> hierarchyComponentList = new List<IHierarchyActiveDependent>();

		//public static implicit operator Node(GameObject gameObject)
		//{
		//    return gameObject.node;
		//}

		//static Dictionary<Node, GameObject> nodeGameObjectDictionary = new Dictionary<Node, GameObject>();
		//public static implicit operator GameObject(Node node)
		//{
		//    if (nodeGameObjectDictionary.TryGetValue(node, out GameObject gameObject))
		//    {
		//        return gameObject;
		//    }
		//    GameObject newGO = new GameObject(node);
		//    nodeGameObjectDictionary.Add(node, newGO);
		//    return newGO;
		//}

		public override void _Ready()
		{
			Node3D node3D = GetNode(".") as Node3D;
			if (node3D == null) { Debug.Log("node3D  == null"); }
			else { Debug.Log("node3D not null"); }
			Debug.Log(node3D.GlobalPosition);

			List<Component> presetComponents = new List<Component>();

			CollectChildNode_excludeGrandchild(this, presetComponents);
			for (int i = 0; i < presetComponents.Count; i++)
			{
				initializeComponent(presetComponents[i]);
			}
		}

		void initializeComponent(Component newComponent)
		{
			newComponent.setGO(this);
			if (newComponent is IHierarchyActiveDependent) hierarchyComponentList.Add(newComponent as IHierarchyActiveDependent);
			if (newComponent is Transform)
			{
				transform = newComponent as Transform;
				transform.findNode3D();

			}
			newComponent.Name = newComponent.GetType().Name;
		}

		public T AddComponent<T>() where T : Component, new()
		{
			Node newNode = new Node();
			AddChild(newNode);
			T newComponent = new T();
			newNode.SetScript(newComponent);
			initializeComponent(newComponent);


			return newComponent;
		}


		public T GetComponent<T>() where T : Component
		{
			return FindChild<T>(this, false);

		}


		public T GetComponentInParent<T>() where T : Component
		{
			Node curNode = this;
			T component = null;
			do
			{
				GameObject parentGO = curNode as GameObject;
				if (parentGO != null)
				{
					component = parentGO.GetComponent<T>();
					if (component != null)
						return component;
				}

				curNode = curNode.GetParent();

			}
			while (curNode != null);

			return null;
		}



		public T[] GetComponentsInParent<T>() where T : Component
		{
			List<T> components = new List<T>();

			Node curNode = this;

			do
			{
				GameObject parentGO = curNode as GameObject;
				if (parentGO != null)
				{
					T component = null;
					component = parentGO.GetComponent<T>();
					if (component != null)
						components.Add(component);
				}

				curNode = curNode.GetParent();

			}
			while (curNode != null);

			return components.ToArray();
		}


		public T GetComponentInChildren<T>() where T : Component
		{
			return FindChild<T>(this);
		}

		public T[] GetComponentsInChildren<T>() where T : Component
		{
			List<T> components = new List<T>();
			CollectChildNode<T>(this, components);
			return components.ToArray();
		}





		public static Node Find(string name)
		{
			Node root = UnityEngineAutoLoad.Instance.GetTree().Root;
			return FindChild(root, name);
		}


		static Node FindChild(Node parent, string name)
		{
			int childCount = parent.GetChildCount();

			if (parent.Name.Equals(name))
			{
				return parent;
			}

			if (childCount > 0)
			{
				for (int i = 0; i < childCount; i++)
				{
					Node node = FindChild(parent.GetChild(i), name);

					if (node != null)
					{
						return node;
					}
				}
			}

			return null;
		}


		static T FindChild<T>(Node parent, bool includeGrandchildren = true) where T : Node
		{
			int childCount = parent.GetChildCount();

			if (parent is T)
			{
				return parent as T;
			}

			if (childCount > 0)
			{
				for (int i = 0; i < childCount; i++)
				{
					T node = null;
					if (includeGrandchildren)
						node = FindChild<T>(parent.GetChild(i));
					else
						node = parent.GetChild(i) as T;
					if (node != null)
					{
						return node;
					}
				}
			}

			return null;
		}

		static void CollectChildNode<T>(Node parent, List<T> nodeOfType) where T : Node
		{
			int childCount = parent.GetChildCount();

			if (parent is T)
			{
				nodeOfType.Add(parent as T);
			}

			if (childCount > 0)
			{
				for (int i = 0; i < childCount; i++)
				{
					CollectChildNode<T>(parent.GetChild(i), nodeOfType);
				}
			}
		}

		static void CollectChildNode_excludeGrandchild<T>(Node parent, List<T> nodeOfType) where T : Node
		{
			int childCount = parent.GetChildCount();
			if (parent is T)
			{
				nodeOfType.Add(parent as T);
			}
			if (childCount > 0)
			{
				for (int i = 0; i < childCount; i++)
				{
					T node = parent.GetChild(i) as T;
					if (node != null)
					{
						nodeOfType.Add(node);
					}
				}
			}
		}

	}

	public static class NodeGameObjectExtension
	{
	   
		public static bool IsGameObject(this Node node)
		{
			return node.HasMeta("GameObject");

		}

	}
}
