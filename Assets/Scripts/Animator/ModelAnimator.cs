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

using Mediapipe;
using UnityEngine;

public class ModelAnimator : MonoBehaviour {
  private Animator _anim;
  private Joint[] _joints = new Joint[Landmarks.Total];
  [SerializeField] private Transform _nose;

  public void Start() {
    _anim = GetComponent<Animator>();

    // Right Arm
    _joints[Landmarks.RightShoulder] = new Joint(_anim.GetBoneTransform(HumanBodyBones.RightUpperArm));
    _joints[Landmarks.RightElbow] = new Joint(_anim.GetBoneTransform(HumanBodyBones.RightLowerArm));
    _joints[Landmarks.RightWrist] = new Joint(_anim.GetBoneTransform(HumanBodyBones.RightHand));
    _joints[Landmarks.RightThumb] = new Joint(_anim.GetBoneTransform(HumanBodyBones.RightThumbIntermediate));
    _joints[Landmarks.RightIndex] = new Joint(_anim.GetBoneTransform(HumanBodyBones.RightMiddleProximal));

    // Left Arm
    _joints[Landmarks.LeftShoulder] = new Joint(_anim.GetBoneTransform(HumanBodyBones.LeftUpperArm));
    _joints[Landmarks.LeftElbow] = new Joint(_anim.GetBoneTransform(HumanBodyBones.LeftLowerArm));
    _joints[Landmarks.LeftWrist] = new Joint(_anim.GetBoneTransform(HumanBodyBones.LeftHand));
    _joints[Landmarks.LeftThumb] = new Joint(_anim.GetBoneTransform(HumanBodyBones.LeftThumbIntermediate));
    _joints[Landmarks.LeftIndex] = new Joint(_anim.GetBoneTransform(HumanBodyBones.LeftMiddleProximal));

    // Face
    _joints[Landmarks.LeftEar] = new Joint(_anim.GetBoneTransform(HumanBodyBones.Head));
    _joints[Landmarks.LeftEye] = new Joint(_anim.GetBoneTransform(HumanBodyBones.LeftEye));
    _joints[Landmarks.RightEar] = new Joint(_anim.GetBoneTransform(HumanBodyBones.Head));
    _joints[Landmarks.RightEye] = new Joint(_anim.GetBoneTransform(HumanBodyBones.RightEye));
    _joints[Landmarks.Nose] = new Joint(_nose.transform);

    // Right Leg
    _joints[Landmarks.RightHip] = new Joint(_anim.GetBoneTransform(HumanBodyBones.RightUpperLeg));
    _joints[Landmarks.RightKnee] = new Joint(_anim.GetBoneTransform(HumanBodyBones.RightLowerLeg));
    _joints[Landmarks.RightAnkle] = new Joint(_anim.GetBoneTransform(HumanBodyBones.RightFoot));
    _joints[Landmarks.RightFootIndex] = new Joint(_anim.GetBoneTransform(HumanBodyBones.RightToes));

    // Left Leg
    _joints[Landmarks.LeftHip] = new Joint(_anim.GetBoneTransform(HumanBodyBones.LeftUpperLeg));
    _joints[Landmarks.LeftKnee] = new Joint(_anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg));
    _joints[Landmarks.LeftAnkle] = new Joint(_anim.GetBoneTransform(HumanBodyBones.LeftFoot));
    _joints[Landmarks.LeftFootIndex] = new Joint(_anim.GetBoneTransform(HumanBodyBones.LeftToes));

    // Misc
    _joints[Landmarks.Hip] = new Joint(_anim.GetBoneTransform(HumanBodyBones.Hips));
    _joints[Landmarks.Head] = new Joint(_anim.GetBoneTransform(HumanBodyBones.Head));
    _joints[Landmarks.Neck] = new Joint(_anim.GetBoneTransform(HumanBodyBones.Neck));
    _joints[Landmarks.Spine] = new Joint(_anim.GetBoneTransform(HumanBodyBones.Spine));

    // Connections
    // Right Arm
    _joints[Landmarks.RightShoulder].Child = _joints[Landmarks.RightElbow];
    _joints[Landmarks.RightElbow].Child = _joints[Landmarks.RightWrist];
    _joints[Landmarks.RightElbow].Parent = _joints[Landmarks.RightShoulder];

    // Left Arm
    _joints[Landmarks.LeftShoulder].Child = _joints[Landmarks.LeftElbow];
    _joints[Landmarks.LeftElbow].Child = _joints[Landmarks.LeftWrist];
    _joints[Landmarks.LeftElbow].Parent = _joints[Landmarks.LeftShoulder];

    // Right Leg
    _joints[Landmarks.RightHip].Child = _joints[Landmarks.RightKnee];
    _joints[Landmarks.RightKnee].Child = _joints[Landmarks.RightAnkle];
    _joints[Landmarks.RightAnkle].Child = _joints[Landmarks.RightFootIndex];
    _joints[Landmarks.RightAnkle].Parent = _joints[Landmarks.RightKnee];

    // Left Leg
    _joints[Landmarks.LeftHip].Child = _joints[Landmarks.LeftKnee];
    _joints[Landmarks.LeftKnee].Child = _joints[Landmarks.LeftAnkle];
    _joints[Landmarks.LeftAnkle].Child = _joints[Landmarks.LeftFootIndex];
    _joints[Landmarks.LeftAnkle].Parent = _joints[Landmarks.LeftKnee];

    // Spine
    _joints[Landmarks.Spine].Child = _joints[Landmarks.Neck];
    _joints[Landmarks.Neck].Child = _joints[Landmarks.Head];

    // Set Inverse
    Vector3 forward = GetNormal(_joints[Landmarks.LeftShoulder].position, _joints[Landmarks.LeftHip].position, _joints[Landmarks.RightHip].position);
    foreach (Joint joint in _joints) {
      if (joint != null && joint.Child != null) {
        joint.Forward = GetLook(joint, joint.Child, forward);
      }
    }
    Joint hip = _joints[Landmarks.Hip];
    hip.Forward = Quaternion.LookRotation(forward);

    // For Head Rotation
    Joint head = _joints[Landmarks.Head];
    Vector3 gaze = _joints[Landmarks.Nose].position - _joints[Landmarks.Head].position;
    head.Forward = Quaternion.LookRotation(gaze);  
  }

  public void Update() {
  }

  public void SetPoseLandmarks(NormalizedLandmarkList poseLandmarks) {
  }

  // <summary>
  // Get the normal to a triangle from the three corner points, a, b and c.
  // See https://docs.unity3d.com/ScriptReference/Vector3.Cross.html.
  // </summary>
  private Vector3 GetNormal(Vector3 a, Vector3 b, Vector3 c) {
    // Find vectors corresponding to two of the sides of the triangle.
    Vector3 side1 = b - a;
    Vector3 side2 = c - a;

    // Cross the vectors to get a perpendicular vector, then normalize it.
    return Vector3.Cross(side1, side2).normalized;
  }

  private Quaternion GetLook(Joint p1, Joint p2, Vector3 forward) {
    return Quaternion.LookRotation(p1.position - p2.position, forward);
  }  
}
