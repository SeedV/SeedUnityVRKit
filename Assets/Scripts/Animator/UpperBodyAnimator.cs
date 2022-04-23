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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mediapipe;
using Mediapipe.Unity;

// <summary>An animator to visualizer upper body and face.</summary>
public class UpperBodyAnimator : MonoBehaviour {
  [Tooltip("Reference to MTH_DEF game object in UnityChan model.")]
  public SkinnedMeshRenderer ref_SMR_MTH_DEF;
  // <summary>The last detection of face landmarks, set by OnFaceLandmarksOutput.</summary>
  private NormalizedLandmarkList _faceLandmarks;
  // <summary>The computed mouth aspect ratio.</summary>
  [Range(0.0f, 1.0f)]
  private float _mar = 0;
  // <summary>The computed mouth distance.</summary>
  private float _mouthDistance = 0;

  void LateUpdate() {
    if (_faceLandmarks != null) {
      IList<Vector2> faceMesh = new List<Vector2>();
      foreach (var landmark in _faceLandmarks.Landmark) {
        faceMesh.Add(new Vector2(landmark.X, landmark.Y));
      }
      ComputeMouth(faceMesh);
      SetMouth(_mar * 100);
    }
  }

  private void SetMouth(float ratio) {
    ref_SMR_MTH_DEF.SetBlendShapeWeight(2, ratio);
  }

  public void OnFaceLandmarksOutput(object stream, OutputEventArgs<NormalizedLandmarkList> eventArgs) {
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
}