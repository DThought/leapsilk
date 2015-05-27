using UnityEngine;
using System.Collections;
using Leap;

public class PaintFinger : SkeletalFinger {
  protected const float MIN_SPEED = 10000;
  
  public VoxelGrid canvas;

  public override void InitFinger() {
    base.InitFinger();

    // TODO: check TipVelocity to ensure movement
    if (GetLeapFinger().IsExtended && GetLeapFinger().TipVelocity.MagnitudeSquared >= MIN_SPEED) {
      canvas.LogPoint(GetLeapFinger().Id, GetTipPosition());
    }
  }
}
