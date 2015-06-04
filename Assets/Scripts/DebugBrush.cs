using UnityEngine;
using System.Collections.Generic;
using Leap;

public class DebugBrush : Brush {
  public override void Paint(VoxelGrid canvas, VectorQueue points) {
    if (points.Count == 0) {
      return;
    }

    canvas.DrawVoxel(points[points.Count - 1], pressure);
  }
}
