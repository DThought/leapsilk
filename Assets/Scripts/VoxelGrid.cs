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
              gridDepth, 0.01F);
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
    Transform cell = GetVoxel(point);

    if (cell) {
      Renderer voxel = cell.GetComponent<Renderer>();

      voxel.material.color = Color.Lerp(voxel.material.color, brushColor,
          pressure);
    }
  }

  /**
   * @brief Draws a line using a 3D equivalent of Bresenham's line algorithm.
   * @param start first point to connect.
   * @param end second point to connect.
   * @param pressure amount of new color to apply.
   */
  public void DrawLine(Vector3 start, Vector3 end, float pressure) {
    try {
      start = ClipLine(start, end);
      end = ClipLine(end, start);

      // TODO: implement 3D Bresenham
    } catch (ArgumentOutOfRangeException e) { }
  }

  /**
   * @brief Finds the voxel at a point.
   * @param position coordinates of the point.
   * @return The voxel object, or null if out of bounds
   */
  public Transform GetVoxel(Vector3 position) {
    position -= transform.position;

    int c = (int) ((position.x + center_x) / cellWidth);
    int l = (int) ((position.y + center_z) / cellDepth);
    int r = (int) ((position.z + center_y) / cellHeight);

    if (c < 0 || l < 0 || r < 0 ||
        c >= gridWidth || l >= gridDepth || r >= gridHeight) {

      return null;
    }

    return cube_grid_[r, c, l];
  }

  /**
   * @brief Adds a point to the position history.
   * @param id the ID of the tracker.
   * @param position the point to add.
   */
  public void LogPoint(int id, Vector3 position) {
    history.Add(position);

    if (history.Count > MAX_HISTORY) {
      history.RemoveAt(0);
    }

    brush.Paint(this, history);
  }

  Vector3 ClipLine(Vector3 point, Vector3 origin) {
    point -= transform.position;
    origin -= transform.position;

    // TODO: implement clipping
    if (point.x + center_x < 0) {
      if (origin.x + center_x < 0) {
        throw new ArgumentOutOfRangeException();
      }


    } else if (point.x + center_x >= canvasWidth) {
      if (origin.x + center_x >= canvasWidth) {
        throw new ArgumentOutOfRangeException();
      }


    }

    if (point.y + center_y < 0) {
      if (origin.y + center_y < 0) {
        throw new ArgumentOutOfRangeException();
      }


    } else if (point.y + center_y >= canvasHeight) {
      if (origin.y + center_y >= canvasWidth) {
        throw new ArgumentOutOfRangeException();
      }


    }

    if (point.z + center_z < 0) {
      if (origin.z + center_z < 0) {
        throw new ArgumentOutOfRangeException();
      }


    } else if (point.z + center_z >= canvasHeight) {
      if (origin.z + center_z >= canvasWidth) {
        throw new ArgumentOutOfRangeException();
      }


    }

    return point + transform.position;
  }
}
