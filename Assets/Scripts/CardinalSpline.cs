using UnityEngine;
using System;

public class CardinalSpline {
  private Matrix4x4 b;
  private float tension;

  /**
   * @brief The tension parameter of the spline.
   */
  public float Tension {
    get {
      return tension;
    }

    set {
      float s = (1 - tension) / 2;

      tension = value;
      b[1, 0] = -s;                       b[1, 2] = s;
      b[2, 0] = s * 2;  b[2, 1] = s - 3;  b[2, 2] = 3 - s * 2;  b[2, 3] = -s;
      b[3, 0] = -s;     b[3, 1] = 2 - s;  b[3, 2] = s - 2;      b[3, 3] = s;
    }
  }

  /**
   * @brief Creates a cardinal spline with tension 0 (Catmull-Rom spline).
   */
  public CardinalSpline() : this(0) {}

  /**
   * @brief Creates a cardinal spline with given tension.
   * @param tension tension parameter of the spline.
   */
  public CardinalSpline(float tension) {
    b = new Matrix4x4();
    b[0, 0] = 0;  b[0, 1] = 1;  b[0, 2] = 0;  b[0, 3] = 0;
                  b[1, 1] = 0;                b[1, 3] = 0;
    Tension = tension;
  }

  /**
   * @brief Interpolates the spline using up to four control points.
   * @param p0 first point.
   * @param p1 second point.
   * @param p2 third point.
   * @param p3 fourth point.
   * @param t interpolation parameter between p1 and p2.
   * @return The spline evaluated at parameter t between p1 and p2
   */
  public Vector3 Lerp(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3,
      float t) {
    Vector4 px = new Vector4(p0.x, p1.x, p2.x, p3.x);
    Vector4 py = new Vector4(p0.y, p1.y, p2.y, p3.y);
    Vector4 pz = new Vector4(p0.z, p1.z, p2.z, p3.z);
    Vector3 p = Vector3.zero;
    float u = 1;

    px = b * px;
    py = b * py;
    pz = b * pz;

    for (int i = 0; i < 4; i++) {
      p.x += u * px[i];
      p.y += u * py[i];
      p.z += u * pz[i];
      u *= t;
    }

    return p;
  }
}
