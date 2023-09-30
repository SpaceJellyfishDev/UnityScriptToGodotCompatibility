using Godot;
using System.Reflection;
using System.Collections.Generic;
using System;
namespace UnityEngine
{
	//[Tool]
	public partial class UnityEngineAutoLoad : Node
	{
		static UnityEngineAutoLoad instance = null;
		public static UnityEngineAutoLoad Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new UnityEngineAutoLoad();
				}

				return instance;
			}
		}


		List<Node> monoNodes = new List<Node>();
		Dictionary<Node, MonoBehaviourController> monoBehaviours = new Dictionary<Node, MonoBehaviourController>();
		bool _log = false;
		[Export] bool log { get=> _log; set { _log = value; Log(); } }
		public UnityEngineAutoLoad()
		{
			instance = this;
		}


		public void Log()
		{
		   
			if (!_log) return;
			Debug.Log("monoNodes count = " + monoNodes.Count);
			_log = false;
		}

		public override void _Ready()
		{
			//GetTree().Connect("node_added", this, "_node_added");
			GetTree().Connect("node_added", Callable.From((Node node) => _node_added(node)));

			InitialScan();
		}


		void InitialScan()
		{
			CheckNode(GetTree().Root);
		}


		void CheckNode(Node currentNode)
		{

			MonoBehaviour monoBehaviour = currentNode as MonoBehaviour;
			Debug.Log("Node: " + currentNode.Name);

			if (monoBehaviour != null && !monoBehaviours.ContainsKey(currentNode))
			{
				monoNodes.Add(currentNode);
				MonoBehaviourController newMBC = new MonoBehaviourController();
				monoBehaviours.Add(currentNode, newMBC);
				newMBC.Initialize(monoBehaviour, currentNode);
			}

			for (int i = 0; i < currentNode.GetChildCount(); i++)
			{
				CheckNode(currentNode.GetChild(i));
			}
		}


		void _node_added(Node node)
		{
			Debug.Log(node.Name + " added");

			CheckNode(node);
		}

		public override void _Process(double delta)
		{
			Time.time += (float)delta;
			Time.deltaTime = (float)delta;
			Log();

			foreach (Node node in monoNodes)
			{
				monoBehaviours[node].Update();
			}
		}


		public override void _PhysicsProcess(double delta)
		{
			Time.fixedDeltaTime = (float)delta;

			foreach (Node node in monoNodes)
			{
				monoBehaviours[node].FixedUpdate();
			}
		}


		internal MonoBehaviourController GetMonoBehaviourController(Node node)
		{
			if (monoBehaviours.ContainsKey(node))
			{
				return monoBehaviours[node];
			}

			return null;
		}


		public void Quit()
		{
			SceneTree tree = GetTree();

			if (tree != null)
			{
				tree.Quit();
			}
		}
	}
}
