using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode]
    //[StructLayout (LayoutKind.Sequential)]//use StructLayout can cause compile error
    public sealed partial class Coroutine : CustomYieldInstruction
	{
		IEnumerator routine;
		  

		public Coroutine (IEnumerator routine)
		{
			this.routine = routine;
		}


		public override bool keepWaiting
		{
			get
			{
				return routine.MoveNext();
			}
		}
	}
}
