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

using Logger = Mediapipe.Logger;  // Disambiguation with UnityEngine.Logger
using UnityEngine;

public class ModelAnimator : MonoBehaviour {
  private const string _TAG = nameof(ModelAnimator);
  private Animator _anim;
  private Joint _hip;

  void Start() {
    _anim = GetComponent<Animator>();
    _hip = new Joint(_anim.GetBoneTransform(HumanBodyBones.Hips));
    // Logger.LogInfo(_TAG, $"Initial hip rotation: {_hip.transform.rotation}");
  }

  void Update() {
    // Test code
    _hip.SetRotation(Quaternion.Euler(0, 0, 45));
  }
}
