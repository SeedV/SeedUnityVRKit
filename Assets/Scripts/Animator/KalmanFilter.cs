using System.Collections.Generic;
using UnityEngine;

namespace SeedUnityVRKit {
  // <summary>
  // Kalman filter implementation for <c>Vector3</c>.
  //
  // A good interactive tutorial for beginners about Kalman filter is
  // https://simondlevy.academic.wlu.edu/kalman-tutorial/.
  //
  // This is a scalar version, only models one variable (position).
  // </summary>
  public class KalmanFilter {
    // Process noise models noise to an "ideal" estimated state transition. The larger the
    // value, the less accurate the new estimation is based on law of physics.
    private readonly float _q;
    // Measure noise models noise in the measurement process. The larger the value is, the
    // less accurate the measure is.
    private readonly float _r;
    // Prediction error is used to update the Kalman gain. It is recursively updated from its
    // last value with Kalman gain.
    // p_k = (1 – g_k) * p_(k−1)
    private float _p = 0.1f;
    // Prediction is recursively updated with
    // x_k = x_(k-1) + g_k * (z_k - x_(k-1))
    // It presents the most likely position of the object at given observation.
    private Vector3 _x;
    // The Kalman gain is used to trade-off between estimated state from last time and current
    // measurement. Kalman gain is 0 means the estimated state is correct. Kalman gain is 1 means
    // the measured value (observation) is correct. It can be updated using prediction errors from
    // last state.
    // g_k = (p_(k-1) + q) / (p_(k-1) + q + r)
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
