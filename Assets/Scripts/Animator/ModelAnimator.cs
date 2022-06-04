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
using UnityEngine.Assertions;

namespace SeedUnityVRKit {
  public class ModelAnimator : MonoBehaviour {
    public float Width = 150;
    public float Height = 150;
    private Animator _anim;
    private Joint[] _joints = new Joint[Landmarks.Total];
    [SerializeField]
    private float _zScale = 0.3f;

    // <summary>
    // Creates the joints and set Child, Parent and Forward of each joint.
    // </summary>
    public void Start() {
      _anim = GetComponent<Animator>();

      // Right Arm
      _joints[Landmarks.RightShoulder] =
          new Joint(_anim.GetBoneTransform(HumanBodyBones.RightUpperArm));
      _joints[Landmarks.RightElbow] =
          new Joint(_anim.GetBoneTransform(HumanBodyBones.RightLowerArm));
      _joints[Landmarks.RightWrist] = new Joint(_anim.GetBoneTransform(HumanBodyBones.RightHand));
      _joints[Landmarks.RightThumb] =
          new Joint(_anim.GetBoneTransform(HumanBodyBones.RightThumbIntermediate));
      _joints[Landmarks.RightIndex] =
          new Joint(_anim.GetBoneTransform(HumanBodyBones.RightMiddleProximal));

      // Left Arm
      _joints[Landmarks.LeftShoulder] =
          new Joint(_anim.GetBoneTransform(HumanBodyBones.LeftUpperArm));
      _joints[Landmarks.LeftElbow] = new Joint(_anim.GetBoneTransform(HumanBodyBones.LeftLowerArm));
      _joints[Landmarks.LeftWrist] = new Joint(_anim.GetBoneTransform(HumanBodyBones.LeftHand));
      _joints[Landmarks.LeftThumb] =
          new Joint(_anim.GetBoneTransform(HumanBodyBones.LeftThumbIntermediate));
      _joints[Landmarks.LeftIndex] =
          new Joint(_anim.GetBoneTransform(HumanBodyBones.LeftMiddleProximal));

      // Face
      _joints[Landmarks.LeftEar] = new Joint(_anim.GetBoneTransform(HumanBodyBones.Head));
      _joints[Landmarks.LeftEye] = new Joint(_anim.GetBoneTransform(HumanBodyBones.LeftEye));
      _joints[Landmarks.RightEar] = new Joint(_anim.GetBoneTransform(HumanBodyBones.Head));
      _joints[Landmarks.RightEye] = new Joint(_anim.GetBoneTransform(HumanBodyBones.RightEye));

      // Right Leg
      _joints[Landmarks.RightHip] = new Joint(_anim.GetBoneTransform(HumanBodyBones.RightUpperLeg));
      _joints[Landmarks.RightKnee] =
          new Joint(_anim.GetBoneTransform(HumanBodyBones.RightLowerLeg));
      _joints[Landmarks.RightAnkle] = new Joint(_anim.GetBoneTransform(HumanBodyBones.RightFoot));
      _joints[Landmarks.RightFootIndex] =
          new Joint(_anim.GetBoneTransform(HumanBodyBones.RightToes));

      // Left Leg
      _joints[Landmarks.LeftHip] = new Joint(_anim.GetBoneTransform(HumanBodyBones.LeftUpperLeg));
      _joints[Landmarks.LeftKnee] = new Joint(_anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg));
      _joints[Landmarks.LeftAnkle] = new Joint(_anim.GetBoneTransform(HumanBodyBones.LeftFoot));
      _joints[Landmarks.LeftFootIndex] = new Joint(_anim.GetBoneTransform(HumanBodyBones.LeftToes));

      // Misc
      _joints[Landmarks.Hip] = new Joint(_anim.GetBoneTransform(HumanBodyBones.Hips));
      _joints[Landmarks.Neck] = new Joint(_anim.GetBoneTransform(HumanBodyBones.Neck));

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

      // Set Inverse
      Vector3 forward =
          GetNormal(_joints[Landmarks.LeftShoulder].position, _joints[Landmarks.LeftHip].position,
                    _joints[Landmarks.RightHip].position);
      foreach (Joint joint in _joints) {
        if (joint != null && joint.Child != null) {
          joint.Forward = Quaternion.LookRotation(joint.position - joint.Child.position, forward);
        }
      }
      Joint hip = _joints[Landmarks.Hip];
      hip.Forward = Quaternion.LookRotation(forward);
    }

    // <summary>
    // Calls SetRotation on each joint by their Predictions.
    // </summary>
    public void Update() {
      Vector3 forward =
          GetNormal(_joints[Landmarks.LeftShoulder].Prediction,
                    _joints[Landmarks.LeftHip].Prediction, _joints[Landmarks.RightHip].Prediction);
      _joints[Landmarks.Hip].SetRotation(Quaternion.LookRotation(forward));

      foreach (Joint jointPoint in _joints) {
        if (jointPoint != null && jointPoint.Child != null) {
          Vector3 jointForward = forward;
          if (jointPoint.Parent != null) {
            jointForward = jointPoint.Parent.Prediction - jointPoint.Prediction;
          }
          jointPoint.SetRotation(Quaternion.LookRotation(
              jointPoint.Prediction - jointPoint.Child.Prediction, jointForward));
        }
      }
    }

    // <summary>
    // Updates each joint on their predicted location. It is called when a new prediction is given.
    // </summary>
    public void SetPoseLandmarks(NormalizedLandmarkList poseLandmarks) {
      for (int i = 0; i < Landmarks.PoseCount; i++) {
        NormalizedLandmark landmark = poseLandmarks.Landmark[i];
        if (_joints[i] != null) {
          // Apply a z-scale since the BlazePose model doesn't generate good z-axis estimates.
          // Multiple a width since the landmark coordinates are normalized to [0, 1].
          _joints[i].Prediction =
              new Vector3(-landmark.X * Width, landmark.Y * Height, landmark.Z * _zScale * Width);
        }
      }
      UpdateSpecialJoints();
    }

    // <summary>
    // Updates special joints that doesn't have direct mappings in the predicted ones.
    // </summary>
    private void UpdateSpecialJoints() {
      // Hip, neck, spine and head are not directly generated by BlazePose. We use simple heuristics
      // to find a good estimate with Unity-Chan. They need adjustment for custom models.
      _joints[Landmarks.Hip].Prediction = Vector3.Lerp(
          _joints[Landmarks.LeftHip].Prediction, _joints[Landmarks.RightHip].Prediction, 0.5f);
      _joints[Landmarks.Neck].Prediction =
          Vector3.Lerp(_joints[Landmarks.LeftShoulder].Prediction,
                       _joints[Landmarks.RightShoulder].Prediction, 0.5f);
    }

    // <summary>
    // Get the normal to a triangle from the three corner points, a, b and c.
    // See https://docs.unity3d.com/ScriptReference/Vector3.Cross.html.
    // </summary>
    // <exception cref="Assertions.AssertionException">
    // An assertion is thrown if the sides from input vectors are parallel.
    // </exception>
    private Vector3 GetNormal(Vector3 a, Vector3 b, Vector3 c) {
      // Find vectors corresponding to two of the sides of the triangle.
      Vector3 side1 = b - a;
      Vector3 side2 = c - a;

      // Cross the vectors to get a perpendicular vector, then normalize it.
      Vector3 result = Vector3.Cross(side1, side2).normalized;
      Assert.AreNotEqual(Vector3.zero, result, "The sides from input vectors are parallel.");
      return result;
    }
  }
}
