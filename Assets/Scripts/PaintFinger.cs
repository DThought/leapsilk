using UnityEngine;
using Leap;

public class PaintFinger : SkeletalFinger {
  protected const float MIN_SPEED = 10000;

  public VoxelGrid canvas;

  public override void InitFinger() {
    base.InitFinger();

    if (GetLeapFinger().IsExtended &&
        GetLeapFinger().TipVelocity.MagnitudeSquared >= MIN_SPEED) {

      canvas.LogPoint(GetLeapFinger().Id, GetTipPosition());
    }
  }
}
