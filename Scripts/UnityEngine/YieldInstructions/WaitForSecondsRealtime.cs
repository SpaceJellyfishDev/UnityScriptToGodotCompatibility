using Godot;

namespace UnityEngine
{
	public partial class WaitForSecondsRealtime : CustomYieldInstruction
	{
		public float finishTime;


		public WaitForSecondsRealtime (float seconds)
		{
			finishTime = Time.realtimeSinceStartup + seconds;
		}


		public override bool keepWaiting
		{
			get
			{
				return finishTime > Time.realtimeSinceStartup;
			}
		}
	}
}
