using System.Collections.Generic;
using UnityEngine;

namespace SeedUnityVRKit {
  // <summary>
  // Kalman filter implementation for <c>Vector3</c>.
  // </summary>
  public class KalmanFilter {
    private readonly float _q;
    private readonly float _r;
    private float _p = 0.1f;
    private Vector3 _x;
    private float _k;

    public KalmanFilter(float q, float r) {
      _q = q;
      _r = r;
    }

    public Vector3 Update(Vector3 measurement) {
      _k = (_p + _q) / (_p + _q + _r);
      _p = _r * (_p + _q) / (_r + _p + _q);
      Vector3 ret = _x + (measurement - _x) * _k;
      return _x = ret;
    }

    public void Reset() {
      _p = 1;
      _x = Vector3.zero;
      _k = 0;
    }
  }
}
