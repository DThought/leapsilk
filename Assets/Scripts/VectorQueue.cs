using UnityEngine;
using System;
using System.Collections.Generic;

public class VectorQueue {
  private List<Vector3> history;
  private int c;

  public int Id { get; set; }

  public int Count {
    get {
      return history.Count;
    }
  }

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

  public Vector3 this[int i] {
    get {
      return history[i];
    }

    set {
      history[i] = value;
    }
  }

  public VectorQueue(int id, int capacity) {
    Id = id;
    Capacity = capacity;
  }

  public void Add(Vector3 value) {
    history.Add(value);

    if (history.Count > c) {
      history.RemoveAt(0);
    }
  }
}
