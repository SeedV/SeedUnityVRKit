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

using UnityEngine;

public class Joint {
  // The initial placement when the app starts.
  // It depends on the scene setup but has nothing to do with the animation.
  public Quaternion InitRotation { get; private set; }
  private Transform _target;

  public Joint(Transform target) {
    _target = target;
    InitRotation = target.rotation;
  }

  // Set the rotation referenced by the Quaternion.
  public void SetRotation(Quaternion rotation) {
    _target.rotation = rotation * InitRotation;
  }

  public void SetRotation(Vector3 lookAt, Vector3 upwards) {
    SetRotation(Quaternion.LookRotation(lookAt, upwards));
  }
}
