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

  // <summary>
  // When set, the current joint will act as a hinge with only two directions of freedom.
  // It will always face towards its child bone.
  // </summary>
  public Quaternion Forward { private get; set; }

  // <summary>
  // The target's position. This should be used for setting up forward vectors and almost should
  // not be used outside of initialization code.
  // </summary>
  public Vector3 position => _target.position;

  public Joint Child;
  public Joint Parent;

  // <summary>
  // The initial placement in the scene.
  // </summary>
  private readonly Quaternion _initRotation;

  // <summary>
  // The transform target.
  // </summary>
  private readonly Transform _target;

  public Joint(Transform target) {
    _target = target;
    if (_target != null) {
      _initRotation = target.rotation;
    }
  }

  // <summary>
  // Set the rotation referenced by the Quaternion.
  // If the forward rotation is set, it would be used to adjust so that the joint will face to
  // its child joint.
  // </summary>
  public void SetRotation(Quaternion rotation) {
    _target.rotation = rotation * Quaternion.Inverse(Forward) * _initRotation;
  }

  // <summary>
  // Set the rotation referenced by forward and upward vectors.
  // If the forward rotation is set, it would be used to adjust so that the joint will face to
  // its child joint.
  // </summary>
  public void SetRotation(Vector3 lookAt, Vector3 upwards) {
    SetRotation(Quaternion.LookRotation(lookAt, upwards));
  }
}
