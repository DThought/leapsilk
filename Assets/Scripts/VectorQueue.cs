using UnityEngine;
using System;
using System.Collections.Generic;

public class VectorQueue {
  protected List<Vector3> history;
  protected int c;

  /**
   * @brief The current number of points in history.
   */
  public int Count {
    get {
      return history.Count;
    }
  }

  /**
   * @brief The number of points to maintain in history.
   */
  public int Capacity {
    get {
      return c;
    }

    set {
      if (c < 0) {
        throw new ArgumentOutOfRangeException();
      }

      c = value;

      while (history.Count > c) {
        history.RemoveAt(0);
      }
    }
  }

  /**
   * @brief The ID of the associated tracker.
   */
  public int Id { get; set; }

  /**
   * @brief Access item indexed from the start of the history.
   */
  public Vector3 this[int i] {
    get {
      return history[i];
    }

    set {
      history[i] = value;
    }
  }

  /**
   * @brief Creates a new VectorQueue.
   * @param id ID of the associated tracker.
   * @param capacity number of points to maintain in history.
   */
  public VectorQueue(int id, int capacity) {
    history = new List<Vector3>();
    Id = id;
    Capacity = capacity;
  }

  /**
   * @brief Adds a point to the end of the history.
   * @param value the point to add.
   */
  public void Add(Vector3 value) {
    history.Add(value);

    if (history.Count > c) {
      history.RemoveAt(0);
    }
  }
}
