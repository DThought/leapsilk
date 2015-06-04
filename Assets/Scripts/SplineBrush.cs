using UnityEngine;
using System.Collections.Generic;
using Leap;

public class SplineBrush : Brush {
  public float tension = 0;
  public int divisions = 4;

  private CardinalSpline spline;
  private float step;

  void Start() {
    spline = new CardinalSpline(tension);
    step = (float) 1 / divisions;
  }

  public override void Paint(VoxelGrid canvas, VectorQueue points) {
    if (points.Count < 2) {
      return;
    }

    Vector3 p0;
    Vector3 p3;
    Vector3 start;
    Vector3 end;

    for (int i = 0; i < points.Count - 1; i++) {
      p0 = points[i == 0 ? 0 : i - 1];
      p3 = points[i == points.Count - 2 ? i + 1 : i + 2];
      start = points[i];

      for (float j = step; j < 1; j += step) {
        end = spline.Lerp(p0, points[i], points[i + 1], p3, j);
        canvas.DrawLine(start, end, pressure);
        start = end;
      }

      canvas.DrawLine(start, points[i + 1], pressure);
    }
  }
}
