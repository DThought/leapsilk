using UnityEngine;
using System;
using System.Collections.Generic;
using Leap;

public class VoxelGrid : MonoBehaviour {
  protected const int MAX_HISTORY = 4;
  protected const int MAX_TRACKERS = 2;

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
  List<VectorQueue> histories;

  void Start() {
    cube_grid_ = new Transform[gridHeight, gridWidth, gridDepth];
    histories = new List<VectorQueue>();

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
          cube_grid_[r, c, l] = Instantiate(
              model,
              transform.position + location,
              transform.rotation * model.transform.rotation) as Transform;
          cube_grid_[r, c, l].localScale = scale;
          cube_grid_[r, c, l].parent = transform;
          cube_grid_[r, c, l].GetComponent<Renderer>().material.color = new
              Color((float) r / gridHeight,
                    (float) c / gridWidth,
                    (float) l / gridDepth,
                    debugPressure);
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
    } catch (IndexOutOfRangeException) {}
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

    // see ftp://ftp.isc.org/pub/usenet/comp.sources.unix/volume26/line3d
    int c1 = (int) ((start.x + center_x) / cellWidth);
    int l1 = (int) ((start.y + center_z) / cellDepth);
    int r1 = (int) ((start.z + center_y) / cellHeight);
    int c2 = (int) ((end.x + center_x) / cellWidth);
    int l2 = (int) ((end.y + center_z) / cellDepth);
    int r2 = (int) ((end.z + center_y) / cellHeight);
    int dc = c2 - c1;
    int dl = l2 - l1;
    int dr = r2 - r1;
    int abs_c = Math.Abs(dc) * 2;
    int abs_l = Math.Abs(dl) * 2;
    int abs_r = Math.Abs(dr) * 2;
    int sign_c = Math.Sign(dc);
    int sign_l = Math.Sign(dl);
    int sign_r = Math.Sign(dr);

    if (abs_c >= abs_l && abs_c >= abs_r) {
      // gradient dominant in direction of c
      dl = abs_l - abs_c / 2;
      dr = abs_r - abs_c / 2;

      while (true) {
        DrawCell(r1, c1, l1, pressure);

        if (c1 == c2) {
          break;
        }

        if (dl >= 0) {
          l1 += sign_l;
          dl -= abs_c;
        }

        if (dr >= 0) {
          r1 += sign_r;
          dr -= abs_c;
        }

        c1 += sign_c;
        dl += abs_l;
        dr += abs_r;
      }
    } else if (abs_l >= abs_c && abs_l >= abs_r) {
      // gradient dominant in direction of l
      dc = abs_c - abs_l / 2;
      dr = abs_r - abs_l / 2;

      while (true) {
        DrawCell(r1, c1, l1, pressure);

        if (l1 == l2) {
          break;
        }

        if (dc >= 0) {
          c1 += sign_c;
          dc -= abs_l;
        }

        if (dr >= 0) {
          r1 += sign_r;
          dr -= abs_l;
        }

        l1 += sign_l;
        dc += abs_c;
        dr += abs_r;
      }
    } else if (abs_r >= abs_l && abs_r >= abs_c) {
      // gradient dominant in direction of r
      dc = abs_c - abs_r / 2;
      dl = abs_l - abs_r / 2;

      while (true) {
        DrawCell(r1, c1, l1, pressure);

        if (r1 == r2) {
          break;
        }

        if (dc >= 0) {
          c1 += sign_c;
          dc -= abs_r;
        }

        if (dl >= 0) {
          l1 += sign_l;
          dl -= abs_r;
        }

        r1 += sign_r;
        dc += abs_c;
        dl += abs_l;
      }
    }
  }

  /**
   * @brief Adds a point to the position history.
   * @param id the ID of the tracker.
   * @param point the point to add.
   */
  public void LogPoint(int id, Vector3 point) {
    int i = 0;

    while (i < histories.Count) {
      if (histories[i] != null && histories[i].Id == id) {
        break;
      }

      i++;
    }

    if (i == histories.Count) {
      histories.Add(new VectorQueue(id, MAX_HISTORY));
    }

    histories[i].Add(point);
    brush.Paint(this, histories[i]);

    if (histories.Count > MAX_TRACKERS) {
      histories.RemoveAt(0);
    }
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
