using UnityEngine;
using System.Collections.Generic;
using Leap;

public class DebugBrush : Brush {
  public override void Paint(VoxelGrid canvas, List<Vector3> points) {
    canvas.DrawVoxel(points[points.Count - 1], pressure);
  }
}
