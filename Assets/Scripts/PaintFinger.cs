using UnityEngine;
using System.Collections;
using Leap;

public class PaintFinger : SkeletalFinger {
	public Color color;
	public VoxelGrid canvas;

	Transform lastVoxel;

	public override void UpdateFinger() {
		base.UpdateFinger();

		// TODO: check TipVelocity to ensure movement
		if (GetLeapFinger().IsExtended) {
			Vector3 tip = GetTipPosition();
			Transform voxel = canvas.GetVoxel(tip);

			if (voxel != lastVoxel) {
				lastVoxel = voxel;
			}
		}

		GUI.Label(new Rect(10, 10, 100, 20), GetTipPosition().ToString());
		GUI.Label(new Rect(10, 30, 100, 20), GetLeapFinger().TipVelocity.ToString());
	}
}
