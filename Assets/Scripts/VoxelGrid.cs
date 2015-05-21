using UnityEngine;
using System.Collections;

public class VoxelGrid : MonoBehaviour {
  public int gridWidth = 25;
  public int gridHeight = 25;
  public int gridDepth = 25;
  public float cellWidth = 1.0f;
  public float cellHeight = 1.0f;
  public float cellDepth = 1.0f;
  public Transform model;
  Transform[,,] cube_grid_;

  void Start() {
    cube_grid_ = new Transform[gridHeight, gridWidth, gridDepth];

    for (int r = 0; r < gridHeight; ++r) {
      for (int c = 0; c < gridWidth; ++c) {
        for (int l = 0; l < gridDepth; ++l) {
          cube_grid_[r, c, l] = transform;
        }
      }
    }

    float center_x = gridWidth * cellWidth / 2.0f;
    float center_y = gridHeight * cellHeight / 2.0f;
    float center_z = gridDepth * cellDepth / 2.0f;

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
