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
using Color = UnityEngine.Color;

using Mediapipe;
using Mediapipe.Unity;

namespace SeedUnityVRKit {
  public enum HandType { LeftHand, RightHand }
  public class HandLandmarksController : MonoBehaviour {
    [Tooltip("Set it to the character's animator game object.")]
    public Animator anim;
    [Tooltip("Left hand or right hand.")]
    public HandType handType;

    [Tooltip("Scale factor to properly convert mediapipe landmarks to world space.")]
    public Vector3 scale = new Vector3(0.2f, 0.2f, 0.2f);

    // The hand root to keep track of the position and rotation.
    private Transform _target;

    private GameObject _indexTip;

    void Start() {
      var bone =
          (handType == HandType.LeftHand) ? HumanBodyBones.RightHand : HumanBodyBones.LeftHand;
      _target = anim.GetBoneTransform(bone);

      _indexTip = new GameObject("IndexTip");
      _indexTip.transform.parent = transform;
    }

    void Update() {
      transform.position = _target.transform.position;
    }

    void OnDrawGizmos() {
      Gizmos.color = Color.red;
      if (_target != null) {
        Gizmos.DrawSphere(_target.position, 0.005f);
      }
      if (_indexTip != null) {
        Gizmos.DrawSphere(_indexTip.transform.position, 0.005f);
      }
    }

    public void OnHandLandmarksOutput(NormalizedLandmarkList landmarkList) {
      if (landmarkList != null) {
        NormalizedLandmark landmark8 = landmarkList.Landmark[8];
        NormalizedLandmark landmark0 = landmarkList.Landmark[0];
        Vector3 tip = Vector3.Scale(new Vector3(landmark8.X, landmark8.Y, landmark8.Z) -
                                        new Vector3(landmark0.X, landmark0.Y, landmark0.Z),
                                    scale);
        _indexTip.transform.localPosition = tip;
      }
    }
  }
}
