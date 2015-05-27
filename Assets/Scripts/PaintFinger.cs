using UnityEngine;
using System.Collections;
using Leap;

public class PaintFinger : SkeletalFinger {
	public VoxelGrid canvas;

	public override void UpdateFinger() {
		base.UpdateFinger();

		if (GetLeapFinger().IsExtended) {
			Vector3 tip = GetTipPosition();
		}
	}
}
