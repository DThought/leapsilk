using UnityEngine;
using Leap;

public class PaintFinger : SkeletalFinger {
  protected const float MIN_SPEED = 10000;

  protected static Color[] colors = {
    Color.red,
    Color.blue,
    Color.green,
    Color.yellow,
    Color.black,
    Color.white
  };

  public VoxelGrid canvas;
  
  private int colorNumber = 0;

  public override void UpdateFinger() {
    base.UpdateFinger();

    if (GetLeapFinger().IsExtended &&
        GetLeapFinger().TipVelocity.MagnitudeSquared >= MIN_SPEED) {
      canvas.LogPoint(GetLeapFinger().Id, GetTipPosition());
    }

    if (!GetLeapFinger().IsExtended) {
      colorNumber = (colorNumber + 1) % colors.Length;
      canvas.brushColor = colors[colorNumber];
    }
  }
}
