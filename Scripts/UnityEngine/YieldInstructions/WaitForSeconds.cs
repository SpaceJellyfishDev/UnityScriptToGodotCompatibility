namespace UnityEngine
{
	public partial class WaitForSeconds : CustomYieldInstruction
	{
		public float finishTime;


		public WaitForSeconds (float seconds)
		{
			finishTime = Time.time + seconds;
		}


		public override bool keepWaiting
		{
			get
			{
				return finishTime > Time.time;
			}
		}
	}
}
