using UnityEngine;
using System.Collections.Generic;
using Leap;

public abstract class Brush : MonoBehaviour {
  public float pressure;

  /**
   * @brief Called to apply this brush.
   * @param canvas the VoxelGrid to paint on.
   * @param points the set of points to stroke.
   */
  public abstract void Paint(VoxelGrid canvas, VectorQueue points);
}
