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

namespace SeedUnityVRKit {
  public enum HandType { LeftHand, RightHand }
  public class HandLandmarksController : MonoBehaviour {
    [Tooltip("Set it to the character's animator game object.")]
    public Animator anim;
    [Tooltip("Left hand or right hand.")]
    public HandType handType;

    // The hand root to keep track of the position and rotation.
    private Transform _target;

    void Start() {
      var bone =
          (handType == HandType.LeftHand) ? HumanBodyBones.LeftHand : HumanBodyBones.RightHand;
      _target = anim.GetBoneTransform(bone);
    }

    void OnDrawGizmos() {
      if (_target != null) {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_target.position, 0.005f);
      }
    }
  }
}
