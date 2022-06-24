using System.Collections.Generic;
using UnityEngine;

namespace SeedUnityVRKit {
  // <summary>
  // Kalman filter implementation for <c>Vector3</c>.
  //
  // A good interactive tutorial for beginners about Kalman filter is
  // https://simondlevy.academic.wlu.edu/kalman-tutorial/.
  //
  // This is a one-dimensional version, only models one variable (position).
  // </summary>
  public class KalmanFilter {
    // Variance of how confident the new estimate follows the prediction.
    // A prediction assumed an ideal state transition following the law of physics.
    private readonly float _q;
    // Variance of how confident the measurement is. The larger this value, the less confident the
    // measurement is.
    private float _r;
    // Variance of how confident the current estimation is.
    // Predict:
    //   pPred = p + q
    // Update:
    //   p = (1 - K) * pPred
    private float _p = 0.1f;
    // The estimated position of the object.
    // Predict:
    //   xPred = x
    // Update:
    //   x = xPred + K * (measurement - xPred)
    private Vector3 _x;
    // The Kalman gain is a scale factor between the predicted location and measurement.
    // Kalman gain is 0 means the predicted location is correct. Kalman gain is 1 means
    // the measured value (observation) is correct.
    // Update:
    //   K = pPred / (pPred + r)
    private float _k;

    public KalmanFilter(float q, float r) {
      _q = q;
      _r = r;
    }

    // <summary>
    // Updates the internal estimation based on measurement. Returns the new estimated position of
    // the object x.
    //
    // Since the predict is trivial in our model, we run predict as a part of the update.
    // Predict:
    //   Calculate xPred (which is the same as x, so no op), and pPred
    // Update:
    //   Update K based on pPred, then apply it to calculate a new x and a new p.
    // </summary>
    public Vector3 Update(Vector3 measurement) {
      var pPred = _p + _q;
      _k = pPred / (pPred + _r);
      _p = (1 - _k) * pPred;
      return _x = _x + (measurement - _x) * _k;
    }

    // <summary>
    // Updates the internal estimation based on measurement. Returns the new estimated position of
    // the object x.
    //
    // This is the same as Update except also accepting a new measurement variance.
    // </summary>
    public Vector3 Update(Vector3 measurement, float r) {
      _r = r;
      return Update(measurement);
    }
  }
}
