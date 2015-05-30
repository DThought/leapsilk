using UnityEngine;
using System.Collections.Generic;
using Leap;

public class StraightBrush : Brush {
  public override void Paint(VoxelGrid canvas, List<Vector3> points) {
    canvas.DrawLine(points[points.Count - 1], points[points.Count - 2],
        pressure);
  }
}
