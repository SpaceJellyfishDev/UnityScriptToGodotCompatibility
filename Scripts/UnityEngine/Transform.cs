using System;
using System.Collections;
using System.Collections.Generic;
using Godot;

namespace UnityEngine
{
	public partial class Transform : Component, IEnumerable
	{
		//public Godot.Spatial spatial;//spatial is renamed to Node3D https://docs.godotengine.org/en/latest/tutorials/3d/introduction_to_3d.html#spatial-node
		public Godot.Node3D gameObjectNode;
		public string name { get { return gameObject.name; } }//set { throw new NotImplementedException(); }

		//internal Transform(Godot.Spatial node)
		internal void findNode3D()
		{
			gameObjectNode = GetParent() as Node3D ;
		}

		public Vector3 position { get { return gameObjectNode.GlobalPosition; } set { gameObjectNode.GlobalPosition = value; } }
		public Vector3 localPosition { get { return gameObjectNode.Position; } set { gameObjectNode.Position = value; } }
		public Quaternion rotation { get { return gameObjectNode.GlobalTransform.Basis.GetRotationQuaternion(); } }//set no equivalent?
		public Quaternion localRotation { get { return gameObjectNode.Quaternion; } set { gameObjectNode.Quaternion = value; } }
		public Vector3 eulerAngles { get { return gameObjectNode.GlobalRotationDegrees; } set { gameObjectNode.GlobalRotationDegrees = value; } }
		public Vector3 localScale { get { return gameObjectNode.Scale; } set { gameObjectNode.Scale = value; } }
		public Vector3 lossyScale { get { return gameObjectNode.GlobalTransform.Basis.Scale; } }

		public Vector3 forward => gameObjectNode.GlobalTransform.Basis.Z;
		public Vector3 up => gameObjectNode.GlobalTransform.Basis.Y;
		public Vector3 right => gameObjectNode.GlobalTransform.Basis.X;

		public Transform parent
		{
			get
			{
				MonoBehaviour parentMono = GetNodeInParent<MonoBehaviour>(gameObject);
				if (parentMono != null) return parentMono.transform;
				else return null;
			}
			set { SetParent(value); }
		}

		public void LookAt(Vector3 position, Vector3? up = null) => gameObjectNode.LookAt(position, up);
		public void Rotate(Vector3 eulerAngles)
		{
			//gameObjectNode.Rotate(eulerAngles.normalized, eulerAngles.magnitude * Mathf.Deg2Rad);
			gameObjectNode.RotateX(eulerAngles.x); gameObjectNode.RotateY(eulerAngles.y); gameObjectNode.RotateZ(eulerAngles.z);
		}

		public void Translate(Vector3 globalTranslation) => gameObjectNode.GlobalTranslate(globalTranslation);

		public Vector3 TransformPoint(Vector3 localPoint) => gameObjectNode.GlobalTransform * localPoint;

		public Vector3 InverseTransformPoint(Vector3 worldPoint) => worldPoint * gameObjectNode.GlobalTransform;//or  node.GlobalTransform.affine_inverse() * worldPoint

		public Vector3 TransformDirection(Vector3 localDirection) => gameObjectNode.Transform.Basis * localDirection;

		public Vector3 InverseTransformDirection(Vector3 localDirection) => localDirection * gameObjectNode.Transform.Basis;

		public Vector3 TransformVector(Vector3 localVector) => TransformPoint(localVector) - position;


		public void SetParent(Transform parent)
		{
			gameObject.Reparent(parent.gameObject);
		}

		public void SetParent(Transform parent, bool worldPositionStays)
		{
			gameObject.Reparent(parent.gameObject, worldPositionStays);
		}



		public IEnumerator GetEnumerator()
		{
			if (subTreeChanged)
				countChildren();
			return (IEnumerator)allChildren;
		}


		bool subTreeChanged = true;

		List<MonoBehaviour> allChildren = new List<MonoBehaviour>();
		void countChildren()
		{
			allChildren.Clear();
			int childNodeCount = GetChildCount();
			for (int i = 0; i < childNodeCount; i++)
			{
				MonoBehaviour childMono = GetChild(i) as MonoBehaviour;
				if (childMono != null) allChildren.Add(childMono);
			}
		}
	}


}
