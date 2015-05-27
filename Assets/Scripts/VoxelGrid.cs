using UnityEngine;
using System.Collections;
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
  public Color brushColor = Color.red   ;
  public float pressure = 0.2F;
  
  float cellWidth;
  float cellHeight;
  float cellDepth;
  float center_x;
  float center_y;
  float center_z;
  Transform[,,] cube_grid_;
  Queue history;

  void Start() {
    cube_grid_ = new Transform[gridHeight, gridWidth, gridDepth];
    history = new Queue();
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

  public void LogPoint(int id, Vector3 position) {
    history.Enqueue(position);

    if (history.Count > MAX_HISTORY) {
      history.Dequeue();
    }

    Transform cell = GetVoxel(position);

    if (cell) {
      Renderer voxel = GetVoxel(position).GetComponent<Renderer>();

      voxel.material.color = Color.Lerp(voxel.material.color, brushColor, pressure);
    }
  }

  Transform GetVoxel(Vector3 position) {
    position -= transform.position;

    int c = (int) ((position.x + center_x) / cellWidth);
    int l = (int) ((position.y + center_z) / cellDepth);
    int r = (int) ((position.z + center_y) / cellHeight);

    if (c < 0 || l < 0 || r < 0 || c >= gridWidth || l >= gridDepth || r >= gridHeight) {
      return null;
    }

    return cube_grid_[r, c, l];
  }

  void Update() {
    // TODO
  }
}
