using UnityEngine;
using System.Collections.Generic;
using Leap;

public abstract class Brush : MonoBehaviour {
  public float pressure;

  public abstract void Paint(VoxelGrid canvas, List<Vector3> points);
}
