using Godot;

namespace UnityEngine
{
	class SpatialVisibilityHandler : VisibilityHandler
	{
		public override bool IsVisible
		{
			get
			{
				return spatial.Visible;
			}
		}

		Node3D spatial;


		public SpatialVisibilityHandler(Node3D spatial)
		{
			this.spatial = spatial;
		}
	}
}
