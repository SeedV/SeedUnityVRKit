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
using Color = UnityEngine.Color;

namespace SeedUnityVRKit {

  // <summary>An animator to visualize upper body and face.</summary>
  public class UpperBodyAnimator : MonoBehaviour {
    [Tooltip("Reference to MTH_DEF game object in UnityChan model.")]
    public SkinnedMeshRenderer MthDefRef;
    [Tooltip("Max rotation angle in degree.")]
    [Range(0, 45f)]
    public float MaxRotationThreshold = 40f;
    [Tooltip("Screen width used as to scale the recognized normalized landmarks.")]
    public float ScreenWidth = 1920;
    [Tooltip("Screen height used as to scale the recognized normalized landmarks.")]
    public float ScreenHeight = 1080;
    /// <summary>The last detection of face landmarks, set by OnFaceLandmarksOutput.</summary>
    private NormalizedLandmarkList _faceLandmarks;
    /// <summary>The computed mouth aspect ratio.</summary>
    [Range(0.0f, 1.0f)]
    private float _mar = 0;
    /// <summary>The computed mouth distance.</summary>
    private float _mouthDistance = 0;
    /// <summary>The neck joint to control head rotation.</summary>
    private Transform _neck;
    /// <summary>The rotation vector for SolvePnP.</summary>
    private float[] _rotationVector = null;
    /// <summary>The translation vector for SolvePnP.</summary>
    private float[] _translationVector = new float[3];
    /// <summary>
    /// Canonical face model from
    /// https://github.com/google/mediapipe/blob/master/mediapipe/modules/face_geometry/data/canonical_face_model.obj
    /// </summary>
    private float[] _face3DPoints;

    [DllImport("opencvplugin")]
    private static extern void solvePnP(float width, float height, float[] objectPointsArray,
                                        float[] imagePointsArray, float[] cameraMatrixArray,
                                        float[] distCoeffsArray, float[] rvec, float[] tvec,
                                        bool useExtrinsicGuess);

    void Start() {
      var anim = GetComponent<Animator>();

      _neck = anim.GetBoneTransform(HumanBodyBones.Neck);
      _face3DPoints = readFace3DPoints();
    }

    void LateUpdate() {
      if (_faceLandmarks != null) {
        IList<Vector2> faceMesh = new List<Vector2>();
        IList<float> pnp = new List<float>();
        foreach (var landmark in _faceLandmarks.Landmark) {
          faceMesh.Add(new Vector2(landmark.X, landmark.Y));
          pnp.Add(landmark.X * ScreenWidth);
          pnp.Add(landmark.Y * ScreenHeight);
        }
        float[] pnpArray = new float[pnp.Count];
        pnp.CopyTo(pnpArray, 0);
        bool useExtrinsicGuess = (_rotationVector != null);
        if (_rotationVector == null) {
          _rotationVector = new float[3];
        }

        solvePnP(ScreenWidth, ScreenHeight, _face3DPoints, pnpArray, null, null, _rotationVector,
                 _translationVector, useExtrinsicGuess);

        var roll = Mathf.Clamp((float)-Degree(_rotationVector[0]), -MaxRotationThreshold,
                               MaxRotationThreshold);
        var yaw = (float)(-Degree(_rotationVector[1]) + 180);
        var pitch = Mathf.Clamp((float)-Degree(_rotationVector[2]), -MaxRotationThreshold,
                                MaxRotationThreshold);
        _neck.localEulerAngles = new Vector3(yaw, roll, pitch);

        ComputeMouth(faceMesh);
        SetMouth(_mar * 100);
      }
    }

    private float Degree(float radian) {
      return 180.0f / (float)Math.PI * radian;
    }

    private void SetMouth(float ratio) {
      MthDefRef.SetBlendShapeWeight(2, ratio);
    }

    public void OnFaceLandmarksOutput(object stream,
                                      OutputEventArgs<NormalizedLandmarkList> eventArgs) {
      _faceLandmarks = eventArgs.value;
    }

    private void ComputeMouth(IList<Vector2> faceMesh) {
      var p1 = faceMesh[78];
      var p2 = faceMesh[81];
      var p3 = faceMesh[13];
      var p4 = faceMesh[311];
      var p5 = faceMesh[308];
      var p6 = faceMesh[402];
      var p7 = faceMesh[14];
      var p8 = faceMesh[178];
      var mar = (float)((p2 - p8).magnitude + (p3 - p7).magnitude + (p4 - p6).magnitude);
      _mouthDistance = (float)(p1 - p5).magnitude;
      mar /= (float)(2 * _mouthDistance + 1e-6);
      _mar = mar;
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
  }
}
