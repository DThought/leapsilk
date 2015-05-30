using UnityEngine;
using System;
using System.Collections.Generic;
using Leap;

public class VoxelGrid : MonoBehaviour {
  protected const int MAX_HISTORY = 4;

  public int gridWidth = 25;
  public int gridHeight = 25;
  public int gridDepth = 25;
  public float canvasWidth = 25;
  public float canvasHeight = 25;
  public float canvasDepth = 25;
  public Transform model;
  public Brush brush;
  public Color brushColor = Color.red;
  public float debugPressure = 0;

  float cellWidth;
  float cellHeight;
  float cellDepth;
  float center_x;
  float center_y;
  float center_z;
  Transform[,,] cube_grid_;
  List<Vector3> history;

  void Start() {
    cube_grid_ = new Transform[gridHeight, gridWidth, gridDepth];
    history = new List<Vector3>();
    center_x = canvasWidth / 2;
    center_y = canvasHeight / 2;
    center_z = canvasDepth / 2;

    cellWidth = canvasWidth / gridWidth;
    cellHeight = canvasHeight / gridHeight;
    cellDepth = canvasDepth / gridDepth;

    Vector3 scale = new Vector3(cellWidth, cellDepth, cellHeight);

    // Setup the cube grid.
    for (int r = 0; r < gridHeight; ++r) {
      for (int c = 0; c < gridWidth; ++c) {
        for (int l = 0; l < gridDepth; ++l) {
          Vector3 location = new Vector3(c * cellWidth - center_x,
                                         l * cellDepth - center_z,
                                         r * cellHeight - center_y);

          cube_grid_[r, c, l] = Instantiate(model, transform.position +
              location, transform.rotation * model.transform.rotation) as
              Transform;

          cube_grid_[r, c, l].localScale = scale;
          cube_grid_[r, c, l].parent = transform;

          cube_grid_[r, c, l].GetComponent<Renderer>().material.color = new
              Color((float) r / gridHeight, (float) c / gridWidth, (float) l /
              gridDepth, debugPressure);
        }
      }
    }
  }

  /**
   * @brief Adjusts the color of the voxel at a point.
   * @param point coordinates of the point.
   * @param pressure amount of new color to apply.
   */
  public void DrawVoxel(Vector3 point, float pressure) {
    point = ToGrid(point);

    try {
      DrawCell((int) point.z, (int) point.x, (int) point.y, pressure);
    } catch (IndexOutOfRangeException) { }
  }

  /**
   * @brief Draws a line using a 3D equivalent of Bresenham's line algorithm.
   * @param start first point to connect.
   * @param end second point to connect.
   * @param pressure amount of new color to apply.
   */
  public void DrawLine(Vector3 start, Vector3 end, float pressure) {
    try {
      start = CanvasClip(start, end) - transform.position;
      end = CanvasClip(end, start) - transform.position;
    } catch (ArgumentOutOfRangeException) {
      return;
    }

    int c1 = (int) ((start.x + center_x) / cellWidth);
    int l1 = (int) ((start.y + center_z) / cellDepth);
    int r1 = (int) ((start.z + center_y) / cellHeight);
    int c2 = (int) ((end.x + center_x) / cellWidth);
    int l2 = (int) ((end.y + center_z) / cellDepth);
    int r2 = (int) ((end.z + center_y) / cellHeight);
    int dc = c2 - c1;
    int dl = l2 - l1;
    int dr = r2 - r1;
    int ac = Math.Abs(dc) * 2;
    int al = Math.Abs(dl) * 2;
    int ar = Math.Abs(dr) * 2;
    int sc = Math.Sign(dc);
    int sl = Math.Sign(dl);
    int sr = Math.Sign(dr);

    if (ac >= al && ac >= ar) {
      dl = al - ac / 2;
      dr = ar - ac / 2;

      while (true) {
        DrawCell(r1, c1, l1, pressure);

        if (c1 == c2) {
          break;
        }

        if (dl >= 0) {
          l1 += sl;
          dl -= ac;
        }

        if (dr >= 0) {
          r1 += sr;
          dr -= ac;
        }

        c1 += sc;
        dl += al;
        dr += ar;
      }
    } else if (al >= ac && al >= ar) {
      dc = ac - al / 2;
      dr = ar - al / 2;

      while (true) {
        DrawCell(r1, c1, l1, pressure);

        if (l1 == l2) {
          break;
        }

        if (dc >= 0) {
          c1 += sc;
          dc -= al;
        }

        if (dr >= 0) {
          r1 += sr;
          dr -= al;
        }

        l1 += sl;
        dc += ac;
        dr += ar;
      }
    } else if (ar >= al && ar >= ac) {
      dc = ac - ar / 2;
      dl = al - ar / 2;

      while (true) {
        DrawCell(r1, c1, l1, pressure);

        if (r1 == r2) {
          break;
        }

        if (dc >= 0) {
          c1 += sc;
          dc -= ar;
        }

        if (dl >= 0) {
          l1 += sl;
          dl -= ar;
        }

        r1 += sr;
        dc += ac;
        dl += al;
      }
    }
  }

  /**
   * @brief Adds a point to the position history.
   * @param id the ID of the tracker.
   * @param point the point to add.
   */
  public void LogPoint(int id, Vector3 point) {
    history.Add(point);

    if (history.Count > MAX_HISTORY) {
      history.RemoveAt(0);
    }

    brush.Paint(this, history);
  }

  /**
   * @brief Clips an endpoint of a line segment within the canvas bounds.
   * @param point the endpoint to clip.
   * @param origin the endpoint to use as reference.
   * @return A point on the line segment guaranteed to be within bounds
   */
  Vector3 CanvasClip(Vector3 point, Vector3 origin) {
    float t;

    point -= transform.position;
    origin -= transform.position;

    if (point.x + center_x < 0) {
      if (origin.x + center_x < 0) {
        throw new ArgumentOutOfRangeException();
      }

      t = (cellWidth - origin.x - center_x) / (point.x - origin.x);
      point = origin + (point - origin) * t;
    } else if (point.x + center_x >= canvasWidth) {
      if (origin.x + center_x >= canvasWidth) {
        throw new ArgumentOutOfRangeException();
      }

      t = (canvasWidth - cellWidth - origin.x - center_x) /
          (point.x - origin.x);

      point = origin + (point - origin) * t;
    }

    if (point.y + center_z < 0) {
      if (origin.y + center_z < 0) {
        throw new ArgumentOutOfRangeException();
      }

      t = (cellDepth - origin.y - center_z) / (point.y - origin.y);
      point = origin + (point - origin) * t;
    } else if (point.y + center_z >= canvasDepth) {
      if (origin.y + center_z >= canvasDepth) {
        throw new ArgumentOutOfRangeException();
      }

      t = (canvasDepth - cellDepth - origin.y - center_z) /
          (point.y - origin.y);

      point = origin + (point - origin) * t;
    }

    if (point.z + center_y < 0) {
      if (origin.z + center_y < 0) {
        throw new ArgumentOutOfRangeException();
      }

      t = (cellHeight - origin.z - center_y) / (point.z - origin.z);
      point = origin + (point - origin) * t;
    } else if (point.z + center_y >= canvasHeight) {
      if (origin.z + center_y >= canvasHeight) {
        throw new ArgumentOutOfRangeException();
      }

      t = (canvasHeight - cellHeight - origin.z - center_y) /
          (point.z - origin.z);

      point = origin + (point - origin) * t;
    }

    return point + transform.position;
  }

  /**
   * @brief Adjusts the color of the voxel.
   * @param point grid coordinates of the voxel.
   * @param pressure amount of new color to apply.
   */
  void DrawCell(int r, int c, int l, float pressure) {
    Renderer voxel = cube_grid_[r, c, l].GetComponent<Renderer>();

    voxel.material.color = Color.Lerp(voxel.material.color, brushColor,
        pressure);
  }

  /**
   * @brief Converts Unity coordinates to grid coordinates.
   * @param position Unity coordinates to convert.
   * @return Grid coordinates containing the input position
   */
  Vector3 ToGrid(Vector3 position) {
    position -= transform.position;
    position.x = (int) ((position.x + center_x) / cellWidth);
    position.y = (int) ((position.y + center_z) / cellDepth);
    position.z = (int) ((position.z + center_y) / cellHeight);
    return position;
  }
}
