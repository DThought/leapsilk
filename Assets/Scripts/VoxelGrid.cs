using UnityEngine;
using System.Collections;

public class VoxelGrid : MonoBehaviour {
  public int gridWidth = 25;
  public int gridHeight = 25;
  public int gridDepth = 25;
  public float canvasWidth = 25;
  public float canvasHeight = 25;
  public float canvasDepth = 25;
  public Transform model;
  float cellWidth;
  float cellHeight;
  float cellDepth;
  Transform[,,] cube_grid_;

  void Start() {
    cube_grid_ = new Transform[gridHeight, gridWidth, gridDepth];

    float center_x = canvasWidth / 2;
    float center_y = canvasHeight / 2;
    float center_z = canvasDepth / 2;

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
              gridDepth, 0.1F);
        }
      }
    }
  }

  void Update() {
    // TODO: update relevant voxels
  }
}
