using Godot;


namespace UnityEngine
{
	public static class Time
	{
		public static float time { get; internal set; }
		public static float realtimeSinceStartup { get { return Godot.Time.GetTicksMsec() / 1000.0f; } }
		public static float timeScale { get { return (float) Engine.TimeScale; } set { Engine.TimeScale = (value); } }
		public static int frameCount { get { return Engine.GetFramesDrawn(); } }
		public static float deltaTime = 0.0f;
		public static float fixedDeltaTime = 0.0f;
	}
}