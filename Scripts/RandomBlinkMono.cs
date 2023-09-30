
using System;
using UnityEngine;

public partial class RandomBlinkMono : MonoBehaviour
{
	[Godot.Export]
	float blinkDistance = 1f;
	[SerializeField] float blinkInterval = 0.3f;
	System.Random rng = new System.Random();
	float lastBlinkTime = 0f;
	void Update()
	{
		if (UnityEngine.Time.time - lastBlinkTime < blinkInterval)
		{
			return;
		}
		if (transform.gameObjectNode == null) { Debug.Log("gameObjectNode == null"); }
		Debug.Log("before translate ");
		Vector3 translation =
			//new Vector3(UnityEngine.Random.Range(-1f, 1), UnityEngine.Random.Range(-1f, 1), UnityEngine.Random.Range(-1f, 1))
			new Vector3(rng.Next(-1000,1000)/1000f, rng.Next(-1000, 1000) / 1000f, rng.Next(-1000, 1000) / 1000f)
			//UnityEngine.Random.insideUnitSphere
			* blinkDistance;

		transform.Translate(translation);
		Vector3 rotate =
			new Vector3(rng.Next(-180, 180) / 180f, rng.Next(-180, 180) / 180f, rng.Next(-180, 180) / 180f);
		transform.Rotate(rotate);

		Debug.Log("Translate " + translation);
		lastBlinkTime = UnityEngine.Time.time;
	}
}
