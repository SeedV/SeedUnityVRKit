// Copyright 2021-2022 The SeedV Lab.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Mediapipe;
using Mediapipe.Unity;
using UnityEngine;

namespace SeedUnityVRKit {
  public class FaceLandmarksRecognizer {
    // Source of truth:
    // https://github.com/tensorflow/tfjs-models/blob/master/face-landmarks-detection/src/tfjs/constants.ts
    private static readonly int[] _leftEyeIndex =
        new int[] {// Lower contour.
                   33, 7, 163, 144, 145, 153, 154, 155, 133,
                   // upper contour (excluding corners).
                   246, 161, 160, 159, 158, 157, 173
        };
    private static readonly int[] _rightEyeIndex =
        new int[] {// Lower contour.
                   263, 249, 390, 373, 374, 380, 381, 382, 362,
                   // Upper contour (excluding corners).
                   466, 388, 387, 386, 385, 384, 398
        };
    private static readonly float _eyeOpenThreshold = 0.33f;
    /// <summary>
    /// Constant canonical face model from
    /// https://github.com/google/mediapipe/blob/master/mediapipe/modules/face_geometry/data/canonical_face_model.obj
    /// </summary>
    private readonly float[] _face3DPoints;
    /// <summary>Screen width used as to scale the recognized normalized landmarks.</summary>
    private readonly float _screenWidth;
    /// <summary>Screen height used as to scale the recognized normalized landmarks.</summary>
    private readonly float _screenHeight;

    /// <summary>The rotation vector for SolvePnP.</summary>
    private float[] _rotationVector = null;
    /// <summary>The translation vector for SolvePnP.</summary>
    private float[] _translationVector = new float[3];
    /// <summary>Exported native C function for SolvePnP.</summary>
#if UNITY_MAC_X64
    [DllImport("opencvpluginOSInter")]
#else
    [DllImport("opencvplugin")]
#endif
    private static extern void solvePnP(float width, float height, float[] objectPointsArray,
                                        float[] imagePointsArray, float[] cameraMatrixArray,
                                        float[] distCoeffsArray, float[] rvec, float[] tvec,
                                        bool useExtrinsicGuess);

    public FaceLandmarksRecognizer(float screenWidth, float screenHeight) {
      _screenWidth = screenWidth;
      _screenHeight = screenHeight;
      _face3DPoints = readFace3DPoints();
    }

    private static float[] readFace3DPoints() {
      TextAsset modelFile = Resources.Load<TextAsset>("face_model");
      string[] data =
          modelFile.text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
      float[] _face3DPoints = new float[data.Length];
      for (int i = 0; i < data.Length; i++) {
        _face3DPoints[i] = Convert.ToSingle(data[i]);
      }
      return _face3DPoints;
    }

    public FaceLandmarks recognize(NormalizedLandmarkList faceLandmarkList) {
      IList<Vector2> faceMesh = new List<Vector2>();
      IList<float> pnp = new List<float>();
      FaceLandmarks faceLandmarks = new FaceLandmarks();
      foreach (var landmark in faceLandmarkList.Landmark) {
        faceMesh.Add(new Vector2(landmark.X, landmark.Y));
        pnp.Add(landmark.X * _screenWidth);
        pnp.Add(landmark.Y * _screenHeight);
      }
      float[] pnpArray = new float[pnp.Count];
      pnp.CopyTo(pnpArray, 0);
      bool useExtrinsicGuess = (_rotationVector != null);
      if (_rotationVector == null) {
        _rotationVector = new float[3];
      }

      solvePnP(_screenWidth, _screenHeight, _face3DPoints, pnpArray, null, null, _rotationVector,
               _translationVector, useExtrinsicGuess);

      Vector3 axis = new Vector3(_rotationVector[0], _rotationVector[1], _rotationVector[2]);
      float theta = (float)(axis.magnitude * 180 / Math.PI);
      faceLandmarks.FaceRotation = Quaternion.AngleAxis(theta, axis);

      faceLandmarks.MouthAspectRatio = ComputeMouth(faceMesh);
      var leftEyeAspectRatio = ComputeEye(faceMesh, _leftEyeIndex);
      var rightEyeAspectRatio = ComputeEye(faceMesh, _rightEyeIndex);
      faceLandmarks.LeftEyeShape =
          leftEyeAspectRatio > _eyeOpenThreshold ? EyeShape.Open : EyeShape.Close;
      faceLandmarks.RightEyeShape =
          rightEyeAspectRatio > _eyeOpenThreshold ? EyeShape.Open : EyeShape.Close;
      return faceLandmarks;
    }

    private static float Degree(float radian) {
      return 180.0f / (float)Math.PI * radian;
    }

    private static float ComputeMouth(IList<Vector2> faceMesh) {
      var p1 = faceMesh[78];
      var p2 = faceMesh[81];
      var p3 = faceMesh[13];
      var p4 = faceMesh[311];
      var p5 = faceMesh[308];
      var p6 = faceMesh[402];
      var p7 = faceMesh[14];
      var p8 = faceMesh[178];
      var mar = (float)((p2 - p8).magnitude + (p3 - p7).magnitude + (p4 - p6).magnitude);
      var mouthDistance = (float)(p1 - p5).magnitude;
      mar /= (float)(2 * mouthDistance + 1e-6);
      return mar;
    }

    private static float ComputeEye(IList<Vector2> faceMesh, int[] index) {
      float total = 0;
      for (int i = 1; i <= 7; i++) {
        total += (faceMesh[index[i]] - faceMesh[index[8 + i]]).magnitude;
      }
      return total / (faceMesh[index[0]] - faceMesh[index[8]]).magnitude / 7;
    }
  }
}
